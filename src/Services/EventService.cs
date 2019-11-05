using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Disqord;
using Disqord.Events;
using Gommon;
using Humanizer;
using Qmmands;
using Qommon.Collections;
using Volte.Commands;
using Volte.Core;
using Volte.Core.Models;
using Volte.Core.Models.EventArgs;

namespace Volte.Services
{
    public sealed class EventService : VolteService
    {
        private readonly LoggingService _logger;
        private readonly DatabaseService _db;
        private readonly AntilinkService _antilink;
        private readonly BlacklistService _blacklist;
        private readonly PingChecksService _pingchecks;
        private readonly CommandService _commandService;
        private readonly CommandsService _commandsService;
        private readonly QuoteService _quoteService;

        private readonly bool _shouldStream =
            !Config.Streamer.ContainsIgnoreCase(" ") || !Config.Streamer.IsNullOrEmpty();

        public EventService(LoggingService loggingService,
            DatabaseService databaseService,
            AntilinkService antilinkService,
            BlacklistService blacklistService,
            PingChecksService pingChecksService,
            CommandService commandService,
            CommandsService commandsService,
            QuoteService quoteService)
        {
            _logger = loggingService;
            _antilink = antilinkService;
            _db = databaseService;
            _blacklist = blacklistService;
            _pingchecks = pingChecksService;
            _commandService = commandService;
            _commandsService = commandsService;
            _quoteService = quoteService;
        }

        public async Task HandleMessageAsync(MessageReceivedEventArgs evnt)
        {
            var context = VolteContext.FromMessageReceivedEventArgs(evnt);
            if (Config.EnabledFeatures.Blacklist)
                await _blacklist.DoAsync(evnt);
            if (Config.EnabledFeatures.Antilink)
                await _antilink.DoAsync(evnt);
            if (Config.EnabledFeatures.PingChecks)
                await _pingchecks.DoAsync(evnt);

            var data = _db.GetData(evnt.Message.Guild);

            var prefixes = new List<string>
            {
                $"{data.Configuration.CommandPrefix} ", $"{evnt.Client.CurrentUser.Mention} "
            };

            if (CommandUtilities.HasAnyPrefix(evnt.Message.Content, prefixes, StringComparison.OrdinalIgnoreCase, out _,
                out var cmd))
            {
                var sw = Stopwatch.StartNew();
                var result = await _commandService.ExecuteAsync(cmd, context);

                if (result is CommandNotFoundResult) return;

                sw.Stop();
                await _commandsService.OnCommandAsync(new CommandCalledEventArgs(result, context, sw));

                if (context.GuildData.Configuration.DeleteMessageOnCommand)
                    if (!await evnt.Message.TryDeleteAsync())
                        _logger.Warn(LogSource.Service, $"Could not act upon the DeleteMessageOnCommand setting for {context.Guild.Name} as the bot is missing the required permission, or another error occured.");
                return;
            }

            await _quoteService.DoAsync(evnt);
        }

        public async Task OnReady(ReadyEventArgs args)
        {
            var guilds = args.Client.Guilds.Count;
            var users = args.Client.Guilds.SelectMany(x => x.Value.Members).DistinctBy(x => x.Value.Id).Count();
            var channels = args.Client.Guilds.SelectMany(x => x.Value.Channels).DistinctBy(x => x.Value.Id).Count();

            _logger.PrintVersion();
            _logger.Info(LogSource.Volte, "Use this URL to invite me to your guilds:");
            _logger.Info(LogSource.Volte, $"{args.Client.GetInviteUrl()}");
            _logger.Info(LogSource.Volte, $"Logged in as {args.Client.CurrentUser}");
            _logger.Info(LogSource.Volte, "Connected to:");
            _logger.Info(LogSource.Volte, $"    {"guild".ToQuantity(guilds)}");
            _logger.Info(LogSource.Volte, $"    {"user".ToQuantity(users)}");
            _logger.Info(LogSource.Volte, $"    {"channel".ToQuantity(channels)}");

            var c = args.Client.Cast<VolteBot>();
            if (!_shouldStream)
            {
                await c.SetPresenceAsync(new LocalActivity(Config.Game, ActivityType.Playing));
                _logger.Info(LogSource.Volte, $"Set {args.Client.CurrentUser.Name}'s game to \"{Config.Game}\".");
            }
            else
            {
                await c.SetPresenceAsync(new LocalActivity(Config.Game, ActivityType.Streaming, Config.FormattedStreamUrl));
                _logger.Info(LogSource.Volte,
                    $"Set {c.CurrentUser.Name}'s activity to \"{ActivityType.Streaming}: {Config.Game}\", at Twitch user {Config.Streamer}.");
            }

            _ = Task.Run(async () =>
            {
                foreach (var guild in c.Guilds.Values)
                {
                    if (Config.BlacklistedOwners.Contains(guild.OwnerId.RawValue))
                    {
                        _logger.Warn(LogSource.Volte,
                            $"Left guild \"{guild.Name}\" owned by blacklisted owner {guild.Owner}.");
                        await guild.LeaveAsync();
                    }

                    _ = _db.GetData(guild); //ensuring all guilds have data available to prevent exceptions later on 
                }
            });

            if (Config.GuildLogging.EnsureValidConfiguration(c, out var channel))
            {
                await new LocalEmbedBuilder()
                    .WithSuccessColor()
                    .WithAuthor(args.Client.GetOwner())
                    .WithDescription(
                        $"Volte {Version.FullVersion} is starting at **{DateTimeOffset.UtcNow.FormatFullTime()}, on {DateTimeOffset.UtcNow.FormatDate()}**!")
                    .SendToAsync(channel);
            }
        }
    }
}
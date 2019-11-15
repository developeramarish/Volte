using System;
using System.Threading.Tasks;
using Disqord;
using Disqord.Events;
using Gommon;
using Volte.Core.Models;

namespace Volte.Services
{
    public sealed class WelcomeService : VolteService
    {
        private readonly DatabaseService _db;
        private readonly LoggingService _logger;

        public WelcomeService(DatabaseService databaseService,
            LoggingService loggingService)
        {
            _db = databaseService;
            _logger = loggingService;
        }

        internal Task JoinAsync(MemberJoinedEventArgs args) 
            => JoinAsync(args.Member);

        internal Task LeaveAsync(MemberLeftEventArgs args) 
            => LeaveAsync(args.User, args.Guild);

        internal async Task JoinAsync(CachedMember member)
        {
            var data = _db.GetData(member.Guild.Id);

            if (!data.Configuration.Welcome.WelcomeDmMessage.IsNullOrEmpty())
                _ = await member.TrySendMessageAsync(data.Configuration.Welcome.FormatDmMessage(member));
            if (data.Configuration.Welcome.WelcomeMessage.IsNullOrEmpty())
                return; //we don't want to send an empty join message

            _logger.Debug(LogSource.Volte,
                "User joined a guild, let's check to see if we should send a welcome embed.");
            var welcomeMessage = data.Configuration.Welcome.FormatWelcomeMessage(member);
            var c = member.Guild.GetTextChannel(data.Configuration.Welcome.WelcomeChannel);

            if (!(c is null))
            {
                await new LocalEmbedBuilder()
                    .WithColor(data.Configuration.Welcome.WelcomeColor)
                    .WithDescription(welcomeMessage)
                    .WithThumbnailUrl(member.GetAvatarUrl())
                    .WithTimestamp(DateTimeOffset.UtcNow)
                    .SendToAsync(c);

                _logger.Debug(LogSource.Volte, $"Sent a welcome embed to #{c.Name}.");
                return;
            }

            _logger.Debug(LogSource.Volte,
                "WelcomeChannel config value resulted in an invalid/nonexistent channel; aborting.");
        }

        internal async Task LeaveAsync(CachedUser user, CachedGuild guild)
        {
            var data = _db.GetData(guild.Id);
            if (data.Configuration.Welcome.LeavingMessage.IsNullOrEmpty()) return;
            _logger.Debug(LogSource.Volte,
                "User left a guild, let's check to see if we should send a leaving embed.");
            var leavingMessage = data.Configuration.Welcome.FormatLeavingMessage(user, guild);
            var c = guild.GetTextChannel(data.Configuration.Welcome.WelcomeChannel);
            if (!(c is null))
            {
                await new LocalEmbedBuilder()
                    .WithColor(data.Configuration.Welcome.WelcomeColor)
                    .WithDescription(leavingMessage)
                    .WithThumbnailUrl(user.GetAvatarUrl())
                    .WithTimestamp(DateTimeOffset.UtcNow)
                    .SendToAsync(c);
                _logger.Debug(LogSource.Volte, $"Sent a leaving embed to #{c.Name}.");
                return;
            }

            _logger.Debug(LogSource.Volte,
                "WelcomeChannel config value resulted in an invalid/nonexistent channel; aborting.");
        }
    }
}
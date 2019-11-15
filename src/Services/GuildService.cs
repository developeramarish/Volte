using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Disqord;
using Disqord.Events;
using Disqord.Rest;
using Volte.Core;
using Volte.Core.Models;
using Gommon;

namespace Volte.Services
{
    public sealed class GuildService : VolteService
    {
        private readonly LoggingService _logger;
        private readonly VolteBot _bot;
        private readonly DatabaseService _db;

        public GuildService(LoggingService loggingService,
            VolteBot bot,
            DatabaseService databaseService)
        {
            _logger = loggingService;
            _bot = bot;
            _db = databaseService;
        }

        public async Task OnJoinAsync(JoinedGuildEventArgs args)
        {
            _logger.Debug(LogSource.Volte, "Joined a guild.");
            if (Config.BlacklistedOwners.Contains(args.Guild.OwnerId.RawValue))
            {
                _logger.Warn(LogSource.Volte,
                    $"Left guild \"{args.Guild.Name}\" owned by blacklisted owner {args.Guild.Owner}.");
                await args.Guild.LeaveAsync();
                return;
            }

            var embed = new LocalEmbedBuilder()
                .WithTitle("Hey there!")
                .WithAuthor(args.Guild.Owner)
                .WithColor(Config.SuccessColor)
                .WithDescription("Thanks for inviting me! Here's some basic instructions on how to set me up.")
                .AddField("Set your admin role", "$adminrole {roleName}", true)
                .AddField("Set your moderator role", "$modrole {roleName}", true)
                .AddField("Permissions", new StringBuilder()
                    .AppendLine("It is recommended to give me admin permission, to avoid any permission errors that may happen.")
                    .AppendLine("You *can* get away with just send messages, ban members, kick members, and the like if you don't want to give me admin.")
                    .ToString())
                .AddField("Support Server", "[Join my support Discord here](https://discord.gg/H8bcFr2)");

            _logger.Debug(LogSource.Volte,
                "Attempting to send the guild owner the introduction message.");
            try
            {
                await embed.SendToAsync(args.Guild.Owner);
                _logger.Error(LogSource.Volte,
                    "Sent the guild owner the introduction message.");
            }
            catch (DiscordHttpException ex) when (ex.HttpStatusCode is HttpStatusCode.Forbidden)
            {
                var c = args.Guild.TextChannels.OrderByDescending(x => x.Value.Position).FirstOrDefault().Value;
                _logger.Error(LogSource.Volte,
                    "Could not DM the guild owner; sending to the upper-most channel instead.");
                if (c != null) await embed.SendToAsync(c);
            }

            if (!Config.GuildLogging.EnsureValidConfiguration(_bot, out var channel))
            {
                _logger.Error(LogSource.Volte, "Invalid guild_logging.guild_id/guild_logging.channel_id configuration. Check your IDs and try again.");
                return;
            }

            var all = args.Guild.Members;
            var users = all.Where(u => !u.Value.IsBot).ToList();
            var bots = all.Where(u => u.Value.IsBot).ToList();

            var e = new LocalEmbedBuilder()
                .WithAuthor(args.Guild.Owner)
                .WithTitle("Joined Guild")
                .AddField("Name", args.Guild.Name, true)
                .AddField("ID", args.Guild.Id, true)
                .WithThumbnailUrl(args.Guild.GetIconUrl())
                .WithTimestamp(DateTimeOffset.UtcNow)
                .AddField("Users", users.Count, true)
                .AddField("Bots", bots.Count, true);

            if (bots.Count > users.Count)
                await channel.SendMessageAsync(
                    $"{_bot.GetOwner().Mention}: Joined a guild with more bots than users.", false,
                    e.WithSuccessColor().Build());
            else
                await e.WithSuccessColor().SendToAsync(channel);
        }

        public async Task OnLeaveAsync(LeftGuildEventArgs args)
        {
            _logger.Debug(LogSource.Volte, "Left a guild.");
            if (!Config.GuildLogging.EnsureValidConfiguration(_bot, out var channel))
            {
                _logger.Error(LogSource.Volte, "Invalid guild_logging.guild_id/guild_logging.channel_id configuration. Check your IDs and try again.");
                return;
            }

            await new LocalEmbedBuilder()
                .WithAuthor(args.Guild.Owner)
                .WithTitle("Left Guild")
                .AddField("Name", args.Guild.Name, true)
                .AddField("ID", args.Guild.Id, true)
                .WithThumbnailUrl(args.Guild.GetIconUrl())
                .WithErrorColor()
                .SendToAsync(channel);
        }

        public Task OnAvailableAsync(GuildAvailableEventArgs args)
        {
            _ = _db.GetData(args.Guild.Id);
            return Task.CompletedTask;
        }
    }
}
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Disqord;
using Gommon;
using Humanizer;
using Qmmands;
using Volte.Commands.Results;
using Volte.Services;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule : VolteModule
    {
        public CommandsService CommandsService { get; set; }

        [Command("Info")]
        [Description("Provides basic information about this instance of Volte.")]
        [Remarks("info")]
        public async Task<ActionResult> InfoAsync()
            => Ok(Context.CreateEmbedBuilder()
                .AddField("Version", Version.FullVersion, true)
                .AddField("Author", $"{await Context.Bot.GetUserAsync(168548441939509248)}, contributors on [GitHub](https://github.com/Ultz/Volte), and members of the Ultz organization.", true)
                .AddField("Language/Library", $"C# 8, Disqord {Version.DisqordVersion}", true)
                .AddField("Guilds", Context.Bot.Guilds.Count, true)
                .AddField("Channels", Context.Bot.Guilds.SelectMany(x => x.Value.Channels).DistinctBy(x => x.Value.Id).Count(), true)
                .AddField("Invite Me", $"`{CommandService.GetCommand("Invite").GetUsage(Context)}`", true)
                .AddField("Uptime", Process.GetCurrentProcess().GetUptime(), true)
                .AddField("Successful Commands", CommandsService.SuccessfulCommandCalls, true)
                .AddField("Failed Commands", CommandsService.FailedCommandCalls, true)
                .WithThumbnailUrl(Context.Bot.CurrentUser.GetAvatarUrl()));

        [Command("UserInfo", "Ui")]
        [Description("Shows info for the mentioned user or yourself if none is provided.")]
        [Remarks("userinfo [user]")]
        public Task<ActionResult> UserInfoAsync(CachedMember user = null)
        {
            user ??= Context.Member;

            return Ok(Context.CreateEmbedBuilder()
                .WithThumbnailUrl(user.GetAvatarUrl())
                .WithTitle("User Info")
                .AddField("User ID", user.Id, true)
                .AddField("Game", user.Presence.Activity?.Name ?? "Nothing", true)
                .AddField("Status", user.Presence.Status, true)
                .AddField("Is Bot", user.IsBot, true)
                .AddField("Account Created",
                    $"{user.Id.CreatedAt.FormatDate()}, {user.Id.CreatedAt.FormatFullTime()}")
                .AddField("Joined This Guild",
                    $"{user.JoinedAt.FormatDate()}, " +
                    $"{user.JoinedAt.FormatFullTime()}"));
        }

        [Command("ServerInfo", "Si", "GuildInfo", "Gi")]
        [Description("Shows some info about the current guild.")]
        [Remarks("serverinfo")]
        public Task<ActionResult> ServerInfoAsync()
        {
            var cAt = Context.Guild.Id.CreatedAt;

            return Ok(Context.CreateEmbedBuilder()
                .WithTitle("Guild Info")
                .WithThumbnailUrl(Context.Guild.GetIconUrl())
                .AddField("Name", Context.Guild.Name)
                .AddField("Created", $"{cAt.Month}.{cAt.Day}.{cAt.Year} ({cAt.Humanize()})")
                .AddField("Region", Context.Guild.VoiceRegionId)
                .AddField("Members", Context.Guild.Members.Count, true)
                .AddField("Roles", Context.Guild.Roles.Count, true)
                .AddField("Category Channels", Context.Guild.CategoryChannels.Count, true)
                .AddField("Voice Channels", Context.Guild.VoiceChannels.Count, true)
                .AddField("Text Channels", Context.Guild.TextChannels.Count, true));
        }

    }
}
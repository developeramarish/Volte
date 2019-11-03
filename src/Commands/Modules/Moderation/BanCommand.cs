using System.Threading.Tasks;
using Disqord;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Core.Models;
using Volte.Core.Models.EventArgs;
using Volte.Commands.Results;
using Gommon;

namespace Volte.Commands.Modules
{
    public sealed partial class ModerationModule : VolteModule
    {
        [Command("Ban")]
        [Description("Bans the mentioned member.")]
        [Remarks("ban {@member} [reason]")]
        [RequireBotGuildPermission(Permission.BanMembers)]
        [RequireGuildModerator]
        public async Task<ActionResult> BanAsync([CheckHierarchy] CachedMember member,
            [Remainder] string reason = "Banned by a Moderator.")
        {
            if (!await member.TrySendMessageAsync(
                embed: Context.CreateEmbed($"You've been banned from **{Context.Guild.Name}** for **{reason}**.")))
            {
                Logger.Warn(LogSource.Volte,
                    $"encountered a 403 when trying to message {member}!");
            }

            await member.BanAsync(reason, 7);
            return Ok($"Successfully banned **{member}** from this guild.", _ =>
                ModLogService.DoAsync(ModActionEventArgs.New
                    .WithDefaultsFromContext(Context)
                    .WithActionType(ModActionType.Ban)
                    .WithTarget(member)
                    .WithReason(reason))
                );
        }
    }
}
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
        [Command("Softban")]
        [Description("Softbans the mentioned member, kicking them and deleting the last x (where x is defined by the daysToDelete parameter) days of messages.")]
        [Remarks("softban {@member} [daysToDelete] [reason]")]
        [RequireBotGuildPermission(Permission.BanMembers | Permission.KickMembers)]
        [RequireGuildModerator]
        public async Task<ActionResult> SoftBanAsync([CheckHierarchy] CachedMember member, int daysToDelete = 1,
            [Remainder] string reason = "Softbanned by a Moderator.")
        {
            if (!await member.TrySendMessageAsync(
                embed: Context.CreateEmbed($"You've been softbanned from **{Context.Guild.Name}** for **{reason}**.")))
            {
                Logger.Warn(LogSource.Volte,
                    $"encountered a 403 when trying to message {member}!");
            }

            await member.BanAsync(reason, daysToDelete == 1 ? 7 : daysToDelete);
            await Context.Guild.UnbanMemberAsync(member.Id);

            return Ok($"Successfully softbanned **{member.Name}#{member.Discriminator}**.", _ =>
                ModLogService.DoAsync(ModActionEventArgs.New
                    .WithDefaultsFromContext(Context)
                    .WithTarget(member)
                    .WithReason(reason))
                );
        }
    }
}
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
        [Command("Kick")]
        [Description("Kicks the given member.")]
        [Remarks("kick {@member} [reason]")]
        [RequireBotGuildPermission(Permission.KickMembers)]
        [RequireGuildModerator]
        public async Task<ActionResult> KickAsync([CheckHierarchy] CachedMember member,
            [Remainder] string reason = "Kicked by a Moderator.")
        {
            if (!await member.TrySendMessageAsync(
                embed: Context.CreateEmbed($"You've been kicked from **{Context.Guild.Name}** for **{reason}**.")))
            {
                Logger.Warn(LogSource.Volte,
                    $"encountered a 403 when trying to message {member}!");
            }

            await member.KickAsync(RestRequestOptions.FromReason(reason));

            return Ok($"Successfully kicked **{member.Name}#{member.Discriminator}** from this guild.", _ =>
                ModLogService.DoAsync(ModActionEventArgs.New
                    .WithDefaultsFromContext(Context)
                    .WithActionType(ModActionType.Kick)
                    .WithTarget(member)
                    .WithReason(reason))
                );
        }
    }
}
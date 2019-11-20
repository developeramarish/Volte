using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Disqord;
using Disqord.Rest;
using Gommon;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Core.Models;
using Volte.Core.Models.EventArgs;
using Volte.Core.Models.Guild;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class ModerationModule : VolteModule
    {
        [Command("Warn", "W")]
        [Description("Warns the target member for the given reason.")]
        [Remarks("warn {member} {reason}")]
        [RequireGuildModerator]
        public async Task<ActionResult> WarnAsync([CheckHierarchy] CachedMember member, [Remainder] string reason)
        {
            Context.GuildData.Extras.Warns.Add(new Warn
            {
                User = member.Id,
                Reason = reason,
                Issuer = Context.User.Id,
                Date = Context.Now
            });
            Db.UpdateData(Context.GuildData);

            if (!await member.TrySendMessageAsync(
                embed: Context.CreateEmbed($"You've been warned in **{Context.Guild.Name}** for **{reason}**.")))
            {
                Logger.Warn(LogSource.Volte,
                    $"encountered a 403 when trying to message {member}!");
            }

            return Ok($"Successfully warned **{member}** for **{reason}**.",
                _ => ModLogService.DoAsync(ModActionEventArgs.New
                    .WithDefaultsFromContext(Context)
                    .WithActionType(ModActionType.Warn)
                    .WithTarget(member)
                    .WithReason(reason))
            );
        }

        [Command("Warns", "Ws")]
        [Description("Shows all the warns for the given member.")]
        [Remarks("warns {member}")]
        [RequireGuildModerator]
        public Task<ActionResult> WarnsAsync(CachedMember member)
        {
            var warns = Context.GuildData.Extras.Warns.Where(x => x.User == member.Id).Take(10);
            return Ok(new StringBuilder()
                .AppendLine(
                    "Showing the last 10 warnings, or less if the member doesn't have 10 yet, or none if the member's record is clean.")
                .AppendLine()
                .AppendLine($"{warns.Select(x => $"**{x.Reason}**, on **{x.Date.FormatDate()}**").Join("\n")}")
                .ToString());
        }

        [Command("ClearWarns", "Cw")]
        [Description("Clears the warnings for the given member.")]
        [Remarks("clearwarns {member}")]
        [RequireGuildModerator]
        public async Task<ActionResult> ClearWarnsAsync(CachedMember member)
        {
            var newWarnList = Context.GuildData.Extras.Warns.Where(x => x.User != member.Id).ToList();
            Context.GuildData.Extras.Warns = newWarnList;
            Db.UpdateData(Context.GuildData);

            if (!await member.TrySendMessageAsync(
                embed: Context.CreateEmbed($"Your warns in **{Context.Guild.Name}** have been cleared. Hooray!")))
            {
                Logger.Warn(LogSource.Volte,
                    $"encountered a 403 when trying to message {member}!");
            }

            return Ok($"Cleared **{newWarnList.Count}** warnings for **{member}**.", _ =>
                ModLogService.DoAsync(ModActionEventArgs.New
                    .WithDefaultsFromContext(Context)
                    .WithActionType(ModActionType.ClearWarns)
                    .WithTarget(member))
            );
        }
    }
}
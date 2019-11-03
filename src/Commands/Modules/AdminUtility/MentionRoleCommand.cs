using System.Threading.Tasks;
using Disqord;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminUtilityModule : VolteModule
    {
        [Command("MentionRole", "Menro", "Mr")]
        [Description(
            "Mentions a role. If it isn't mentionable, it allows it to be, mentions it, and then undoes the first action.")]
        [Remarks("mentionrole {role}")]
        [RequireBotGuildPermission(Permission.ManageRoles)]
        [RequireGuildAdmin]
        public Task<ActionResult> MentionRoleAsync([Remainder] CachedRole role)
        {
            if (role.IsMentionable)
                return Ok(role.Mention, shouldEmbed: false);

            return Ok(async () =>
            {
                await role.ModifyAsync(x => x.IsMentionable = true);
                await Context.ReplyAsync(role.Mention);
                await role.ModifyAsync(x => x.IsMentionable = false);
            });
        }
    }
}
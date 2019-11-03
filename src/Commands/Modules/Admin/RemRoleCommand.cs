using System.Threading.Tasks;
using Disqord;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminModule : VolteModule
    {
        [Command("RemRole", "Rr")]
        [Description("Remove a role from the mentioned user.")]
        [Remarks("remrole {@user} {roleName}")]
        [RequireGuildAdmin]
        public async Task<ActionResult> RemRoleAsync(CachedMember user, [Remainder] CachedRole role)
        {
            if (role.Position > Context.Guild.CurrentMember.Hierarchy)
            {
                return BadRequest("Role position is too high for me to be able to remove it from anyone.");
            }

            await user.RevokeRoleAsync(role.Id);
            return Ok($"Removed the role **{role.Name}** from {user.Mention}!");
        }
    }
}
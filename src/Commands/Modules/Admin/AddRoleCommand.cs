using System.Threading.Tasks;
using Disqord;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminModule : VolteModule
    {
        [Command("AddRole", "Ar")]
        [Description("Grants a role to the mentioned user.")]
        [Remarks("addrole {@user} {roleName}")]
        [RequireGuildAdmin, RequireBotGuildPermission(Permission.ManageRoles)]
        public async Task<ActionResult> AddRoleAsync(CachedMember member, [Remainder] CachedRole role)
        {
            if (role.Position > Context.Guild.CurrentMember.Hierarchy)
                return BadRequest("Role position is too high for me to be able to grant it to anyone.");

            await member.GrantRoleAsync(role.Id);
            return Ok($"Added the role **{role}** to {member.Mention}!");
        }
    }
}
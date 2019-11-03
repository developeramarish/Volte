using System.Threading.Tasks;
using Disqord;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminUtilityModule : VolteModule
    {
        [Command("RoleColor", "RoleClr", "Rcl")]
        [Description("Changes the color of a specified role. Accepts a Hex or RGB value.")]
        [Remarks("rolecolor {role} {color}")]
        [RequireBotGuildPermission(Permission.ManageRoles)]
        [RequireGuildAdmin]
        public async Task<ActionResult> RoleColorAsync(CachedRole role, [Remainder] Color color)
        {
            await role.ModifyAsync(x => x.Color = color);
            return Ok($"Successfully changed the color of the role **{role.Name}**.");
        }
    }
}
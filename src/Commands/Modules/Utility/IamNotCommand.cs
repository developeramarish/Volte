using System.Linq;
using System.Threading.Tasks;
using Disqord;
using Gommon;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule : VolteModule
    {
        [Command("IamNot")]
        [Description("Take a role from yourself, if it is in the current guild's self role list.")]
        [Remarks("iamnot {roleName}")]
        public async Task<ActionResult> IamNotAsync([Remainder] CachedRole role)
        {
            var d = Db.GetData(Context.Guild.Id);
            if (!d.Extras.SelfRoles.Any(x => x.EqualsIgnoreCase(role.Name)))
            {
                return BadRequest($"The role **{role.Name}** isn't in the self roles list for this guild.");
            }

            var target = Context.Guild.Roles.FirstOrDefault(x => x.Value.Name.EqualsIgnoreCase(role.Name)).Value;
            if (target is null)
            {
                return BadRequest($"The role **{role.Name}** doesn't exist in this guild.");
            }

            await Context.Member.RevokeRoleAsync(target.Id);
            return Ok($"Took away your **{role.Name}** role.");
        }
    }
}
using System.Linq;
using System.Threading.Tasks;
using Gommon;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule : VolteModule
    {
        [Command("SelfRoleList", "Srl")]
        [Description("Gets a list of self roles available for this guild.")]
        [Remarks("selfrolelist")]
        public Task<ActionResult> SelfRoleListAsync()
        {
            var d = Db.GetData(Context.Guild.Id);
            if (d.Extras.SelfRoles.Count <= 0)
                return BadRequest("No roles available to self-assign in this guild.");

            var roleList = d.Extras.SelfRoles.Select(x =>
                {
                    var currentRole = Context.Guild.Roles.FirstOrDefault(r => r.Value.Name.EqualsIgnoreCase(x)).Value;
                    return currentRole is null ? "" : $"**{currentRole.Name}**";
                }).Where(x => !x.IsNullOrEmpty()).Join("\n");

            return Ok(Context.CreateEmbedBuilder(roleList).WithTitle("Roles available to self-assign in this server:"));
        }
    }
}
using System.Threading.Tasks;
using Disqord;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminModule : VolteModule
    {
        [Command("Autorole")]
        [Description("Sets the role to be used for Autorole.")]
        [Remarks("autorole {roleName}")]
        [RequireGuildAdmin]
        public Task<ActionResult> AutoroleAsync([Remainder] CachedRole role)
        {
            Db.ModifyData(Context.Guild, data => data.Configuration.Autorole = role.Id);
            return Ok($"Successfully set **{role.Name}** as the role to be given to members upon joining this guild.");
        }
    }
}
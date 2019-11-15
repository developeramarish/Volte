using System.Threading.Tasks;
using Disqord;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminModule : VolteModule
    {
        [Command("ModRole")]
        [Description("Sets the role able to use Moderation commands for the current guild.")]
        [Remarks("modrole {role}")]
        [RequireGuildAdmin]
        public Task<ActionResult> ModRoleAsync([Remainder] CachedRole role)
        {
            Db.ModifyData(Context.Guild, data => data.Configuration.Moderation.ModRole = role.Id);
            return Ok($"Set **{role.Name}** as the Moderator role for this guild.");
        }
    }
}
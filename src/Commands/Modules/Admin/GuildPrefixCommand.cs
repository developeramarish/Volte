using System.Threading.Tasks;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminModule : VolteModule
    {
        [Command("GuildPrefix", "Gp", "ServerPrefix", "Sp")]
        [Description("Sets the command prefix for this guild.")]
        [Remarks("guildprefix {newPrefix}")]
        [RequireGuildAdmin]
        public Task<ActionResult> GuildPrefixAsync([Remainder] string newPrefix)
        {
            Db.ModifyData(Context.Guild, data => data.Configuration.CommandPrefix = newPrefix);
            return Ok($"Set this guild's prefix to **{newPrefix}**.");
        }
    }
}
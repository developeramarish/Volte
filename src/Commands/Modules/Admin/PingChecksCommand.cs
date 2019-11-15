using System.Threading.Tasks;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminModule : VolteModule
    {
        [Command("PingChecks")]
        [Description("Enable/Disable checking for @everyone and @here for this guild.")]
        [Remarks("pingchecks {true|false}")]
        [RequireGuildAdmin]
        public Task<ActionResult> PingChecksAsync(bool enabled)
        {
            Db.ModifyData(Context.Guild, data => data.Configuration.Moderation.MassPingChecks = enabled);
            return Ok(enabled ? "MassPingChecks has been enabled." : "MassPingChecks has been disabled.");
        }
    }
}
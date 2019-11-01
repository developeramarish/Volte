using System.Text;
using System.Threading.Tasks;
using Disqord;
using Gommon;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule : VolteModule
    {
        [Command("Snowflake")]
        [Description("Shows when the object with the given Snowflake ID was created, in UTC.")]
        [Remarks("snowflake {id}")]
        public Task<ActionResult> SnowflakeAsync(ulong id)
        {
            var s = new Snowflake(id);
            return Ok(new StringBuilder()
                .AppendLine($"**Date:** {s.CreatedAt.FormatDate()}")
                .AppendLine($"**Time**: {s.CreatedAt.FormatFullTime()}")
                .ToString());
        }
    }
}
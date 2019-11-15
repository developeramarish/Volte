using System.Threading.Tasks;
using Disqord;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminModule : VolteModule
    {
        [Command("ModLog")]
        [Description("Sets the channel to be used for mod log.")]
        [Remarks("modlog {channel}")]
        [RequireGuildAdmin]
        public Task<ActionResult> ModLogAsync(CachedTextChannel c)
        {
            Db.ModifyData(Context.Guild, data => data.Configuration.Moderation.ModActionLogChannel = c.Id);
            return Ok($"Set {c.Mention} as the channel to be used by mod log.");
        }
    }
}
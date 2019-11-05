using System.Linq;
using System.Threading.Tasks;
using Disqord;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Commands.Results;
using Gommon;

namespace Volte.Commands.Modules
{
    public sealed partial class ModerationModule : VolteModule
    {
        [Command("Bans")]
        [Description("Shows all bans in this guild.")]
        [Remarks("bans")]
        [RequireBotGuildPermission(Permission.BanMembers)]
        [RequireGuildModerator]
        public async Task<ActionResult> BansAsync()
        {
            var banList = await Context.Guild.GetBansAsync();
            return !banList.Any() 
                ? BadRequest("This guild doesn't have anyone banned.") 
                : Ok(banList.Select(b => $"**{b.User}**: `{b.Reason ?? "No reason provided."}`").Join('\n'));
        }
    }
}
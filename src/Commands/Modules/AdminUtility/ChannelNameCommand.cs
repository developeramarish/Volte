using System.Threading.Tasks;
using Disqord;
using Gommon;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminUtilityModule : VolteModule
    {
        [Command("ChannelName", "Cn")]
        [Description("Sets the name of the current channel.")]
        [Remarks("channelname {name}")]
        [RequireBotChannelPermission(Permission.ManageChannels)]
        [RequireGuildAdmin]
        public async Task<ActionResult> ChannelNameAsync([Remainder] string name)
        {
            await Context.Channel.Cast<CachedTextChannel>().ModifyAsync(x => x.Name = name);
            return Ok($"Set this channel's name to **{name}**.");
        }
    }
}
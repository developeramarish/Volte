using System.Threading.Tasks;
using Disqord;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class BotOwnerModule : VolteModule
    {
        [Command("SetStream")]
        [Description("Sets the bot's stream.")]
        [Remarks("setstream {streamer} {streamName}")]
        [RequireBotOwner]
        public Task<ActionResult> SetStreamAsync(string streamer, [Remainder] string streamName)
            => Ok(
                $"Set the bot's stream to **{streamName}**, and the Twitch URL to **[{streamer}](https://twitch.tv/{streamer})**.",
                _ => Context.Bot.SetPresenceAsync(new LocalActivity(streamName, ActivityType.Streaming, $"https://twitch.tv/{streamer}")));
    }
}
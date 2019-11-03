using System.Text;
using System.Threading.Tasks;
using Disqord;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class BotOwnerModule : VolteModule
    {
        [Command("SetStatus")]
        [Description("Sets the bot's status.")]
        [Remarks("setstatus {dnd|idle|invisible|online}")]
        [RequireBotOwner]
        public Task<ActionResult> SetStatusAsync([Remainder] string status)
            => status.ToLower() switch
            {
                "dnd" => Ok("Set the status to Do Not Disturb.",
                    _ => Context.Bot.SetPresenceAsync(UserStatus.DoNotDisturb)),
                "idle" => Ok("Set the status to Idle.", _ => Context.Bot.SetPresenceAsync(UserStatus.Idle)),
                "invisible" => Ok("Set the status to Invisible.",
                    _ => Context.Bot.SetPresenceAsync(UserStatus.Invisible)),
                "online" => Ok("Set the status to Online.",
                    _ => Context.Bot.SetPresenceAsync(UserStatus.Online)),
                _ => BadRequest(new StringBuilder()
                    .AppendLine("Your option wasn't known, so I didn't modify the status.")
                    .AppendLine("Available options for this command are `dnd`, `idle`, `invisible`, or `online`.")
                    .ToString())
            };
    }
}
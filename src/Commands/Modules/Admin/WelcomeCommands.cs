using System.Text;
using System.Threading.Tasks;
using Disqord;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Commands.Results;
using Volte.Services;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminModule : VolteModule
    {
        public WelcomeService WelcomeService { get; set; }

        [Command("WelcomeChannel", "Wc")]
        [Description("Sets the channel used for welcoming new users for this guild.")]
        [Remarks("welcomechannel {#channel}")]
        [RequireGuildAdmin]
        public Task<ActionResult> WelcomeChannelAsync([Remainder] CachedTextChannel channel)
        {
            Db.ModifyData(Context.Guild, data => data.Configuration.Welcome.WelcomeChannel = channel.Id);
            return Ok($"Set this guild's welcome channel to {channel.Mention}.");
        }

        [Command("WelcomeMessage", "Wmsg")]
        [Description(
            "Sets or shows the welcome message used to welcome new users for this guild. Only in effect when the bot isn't using the welcome image generating API.")]
        [Remarks("welcomemessage [message]")]
        [RequireGuildAdmin]
        public Task<ActionResult> WelcomeMessageAsync([Remainder] string message = null)
        {
            var d = Db.GetData(Context.Guild.Id);
            if (message is null)
            {
                return Ok(new StringBuilder()
                    .AppendLine("The current welcome message for this guild is: ```")
                    .AppendLine(d.Configuration.Welcome.WelcomeMessage)
                    .Append("```")
                    .ToString());
            }

            Db.ModifyData(Context.Guild, data => data.Configuration.Welcome.WelcomeMessage = message);

            var welcomeChannel = Context.Guild.GetTextChannel(d.Configuration.Welcome.WelcomeChannel);
            var sendingTest = d.Configuration.Welcome.WelcomeChannel is 0 || welcomeChannel is null
                ? "Not sending a test message as you do not have a welcome channel set." +
                  "Set a welcome channel to fully complete the setup!"
                : $"Sending a test message to {welcomeChannel.Mention}.";
            if (welcomeChannel is null)
                return Ok(new StringBuilder()
                    .AppendLine($"Set this guild's welcome message to ```{message}```")
                    .AppendLine()
                    .AppendLine(sendingTest)
                    .ToString());

            return Ok(new StringBuilder()
                    .AppendLine($"Set this guild's welcome message to ```{message}```")
                    .AppendLine()
                    .AppendLine($"{sendingTest}").ToString(),
                async _ => await WelcomeService.JoinAsync(Context.Member));
        }

        [Command("WelcomeColor", "WelcomeColour", "Wcl")]
        [Description("Sets the color used for welcome embeds for this guild.")]
        [Remarks("welcomecolor {color}")]
        [RequireGuildAdmin]
        public Task<ActionResult> WelcomeColorAsync([Remainder] Color color)
        {
            Db.ModifyData(Context.Guild, data => data.Configuration.Welcome.WelcomeColor = color.RawValue);
            return Ok("Successfully set this guild's welcome message embed color!");
        }

        [Command("LeavingMessage", "Lmsg")]
        [Description("Sets or shows the leaving message used to say bye for this guild.")]
        [Remarks("leavingmessage [message]")]
        [RequireGuildAdmin]
        public Task<ActionResult> LeavingMessageAsync([Remainder] string message = null)
        {
            var d = Db.GetData(Context.Guild.Id);
            if (message is null)
            {
                return Ok(new StringBuilder()
                    .AppendLine("The current leaving message for this guild is ```")
                    .AppendLine(d.Configuration.Welcome.LeavingMessage)
                    .Append("```")
                    .ToString());
            }

            Db.ModifyData(Context.Guild, data => data.Configuration.Welcome.LeavingMessage = message);
            var welcomeChannel = Context.Guild.GetTextChannel(d.Configuration.Welcome.WelcomeChannel);
            var sendingTest = d.Configuration.Welcome.WelcomeChannel == 0 || welcomeChannel is null
                ? "Not sending a test message, as you do not have a welcome channel set. " +
                  "Set a welcome channel to fully complete the setup!"
                : $"Sending a test message to {welcomeChannel.Mention}.";
            if (welcomeChannel is null)
                return Ok(new StringBuilder()
                    .AppendLine($"Set this guild's leaving message to ```{message}```")
                    .AppendLine()
                    .AppendLine(sendingTest)
                    .ToString());

            return Ok(new StringBuilder()
                    .AppendLine($"Set this guild's leaving message to ```{message}```")
                    .AppendLine()
                    .AppendLine(sendingTest)
                    .ToString(),
                async _ => await WelcomeService.LeaveAsync(Context.User, Context.Guild));
        }

        [Command("WelcomeDmMessage", "Wdmm")]
        [Description("Sets the message to be (attempted to) sent to members upon joining.")]
        [Remarks("welcomedmmessage")]
        [RequireGuildAdmin]
        public Task<ActionResult> WelcomeDmMessageAsync(string message = null)
        {
            var d = Db.GetData(Context.Guild.Id);
            if (message is null)
            {
                return Ok(
                    $"The current WelcomeDmMessage is: ```{d.Configuration.Welcome.WelcomeDmMessage}```");
            }

            Db.ModifyData(Context.Guild, data => data.Configuration.Welcome.WelcomeDmMessage = message);
            return Ok($"Set the WelcomeDmMessage to: ```{message}```");
        }
    }
}
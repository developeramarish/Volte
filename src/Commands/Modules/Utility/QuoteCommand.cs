using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Disqord;
using Disqord.Rest;
using Humanizer;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule : VolteModule
    {
        [Command("Quote"), Priority(0)]
        [Description("Quotes a user from a given message's ID.")]
        [Remarks("quote {messageId}")]
        public async Task<ActionResult> QuoteAsync(ulong messageId)
        {
            var m = await Context.Channel.GetMessageAsync(messageId);
            if (m is null)
                return BadRequest("A message with that ID doesn't exist in this channel.");

            var e = Context.CreateEmbedBuilder(new StringBuilder()
                .AppendLine($"{m.Content}")
                .AppendLine()
                .AppendLine($"[Jump!]({GetJumpUrl(m)})")
                .ToString())
                .WithAuthor($"{m.Author}, in #{Context.Channel.Name}",
                    m.Author.GetAvatarUrl())
                .WithFooter(m.Id.CreatedAt.Humanize());

            return Ok(e);
        }

        [Command("Quote"), Priority(1)]
        [Description("Quotes a user in a different chanel from a given message's ID.")]
        [Remarks("quote {channel} {messageId}")]
        public async Task<ActionResult> QuoteAsync(CachedTextChannel channel, ulong messageId)
        {
            var m = await channel.GetMessageAsync(messageId);
            if (m is null)
                return BadRequest("A message with that ID doesn't exist in the given channel.");

            var e = Context.CreateEmbedBuilder(new StringBuilder()
                    .AppendLine($"{m.Content}")
                    .AppendLine()
                    .AppendLine($"[Jump!]({GetJumpUrl(m, channel)})")
                    .ToString())
                .WithAuthor($"{m.Author}, in #{channel.Name}",
                    m.Author.GetAvatarUrl())
                .WithFooter(m.Id.CreatedAt.Humanize());

            return Ok(e);
        }

        private string GetJumpUrl(RestMessage m, CachedTextChannel channel = null)
        {
            return channel is null 
                ? $"https://discordapp.com/{Context.Guild.Id}/{Context.Channel.Id}/{m.Id}" 
                : $"https://discordapp.com/{channel.Guild.Id}/{channel.Id}/{m.Id}";
        }
    }
}
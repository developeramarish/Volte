using System.Threading.Tasks;
using Disqord;
using Gommon;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule : VolteModule
    {
        [Command("BigEmoji", "HugeEmoji")]
        [Description("Shows the image URL for a given emoji.")]
        [Remarks("bigemoji {emoji}")]
        public Task<ActionResult> BigEmojiAsync(IEmoji emoteIn)
        {
            var url = emoteIn.GetUnicodeUrl();

            return emoteIn switch
            {
                CustomEmoji emote => Ok(Context.CreateEmbedBuilder(emote.GetUrl()).WithImageUrl(emote.GetUrl())),
                Emoji _ => Ok(Context.CreateEmbedBuilder(url).WithImageUrl(url)),
                _ => None() //should never be reached
            };
        }
    }
}
using System.Threading.Tasks;
using Qmmands;
using SixLabors.ImageSharp.PixelFormats;
using Volte.Commands.Results;
using Volte.Helpers;
using System.Text;
using Disqord;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule
    {
        [Command("ShowColor", "Sc")]
        [Description("Shows an image purely made up of the specified color.")]
        [Remarks("showcolor")]
        public Task<ActionResult> ShowColorAsync([Remainder] Color color)
            => Ok(async () =>
            {
                await using (var outStream = ImageHelper.CreateColorImage(new Rgba32(color.R, color.G, color.B)))
                using (var attachment = new LocalAttachment(outStream, "role.png"))
                {
                    await Context.Channel.SendMessageAsync(attachment, embed: new LocalEmbedBuilder()
                        .WithColor(color)
                        .WithTitle("Role Color")
                        .WithDescription(new StringBuilder()
                            .AppendLine($"**Hex:** {color}")
                            .AppendLine($"**RGB:** {color.R}, {color.G}, {color.B}")
                            .ToString())
                        .WithImageUrl("attachment://role.png")
                        .WithTimestamp(Context.Now)
                        .Build());
                }
            });
    }
}

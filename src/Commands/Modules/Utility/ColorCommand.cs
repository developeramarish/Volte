using System.Text;
using System.Threading.Tasks;
using Disqord;
using Qmmands;
using SixLabors.ImageSharp.PixelFormats;
using Volte.Commands.Results;
using Volte.Helpers;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule : VolteModule
    {
        [Command("Color", "Colour")]
        [Description("Shows the Hex and RGB representation for a given role in the current guild.")]
        [Remarks("color {role}")]
        public Task<ActionResult> RoleColorAsync([Remainder] CachedRole role)
        {
            if (!role.Color.HasValue) return BadRequest("Role does not have a color.");

            return Ok(async () =>
            {
                await using (var outStream = ImageHelper.CreateColorImage(new Rgba32(role.Color.Value.R, role.Color.Value.G, role.Color.Value.B)))
                using (var attachment = new LocalAttachment(outStream, "role.png"))
                {
                    await Context.Channel.SendMessageAsync(attachment, embed: new LocalEmbedBuilder()
                        .WithColor(role.Color)
                        .WithTitle("Role Color")
                        .WithDescription(new StringBuilder()
                            .AppendLine($"**Hex:** {role.Color}")
                            .AppendLine($"**RGB:** {role.Color.Value.R}, {role.Color.Value.G}, {role.Color.Value.B}")
                            .ToString())
                        .WithImageUrl("attachment://role.png")
                        .WithTimestamp(Context.Now)
                        .Build());
                }
            });
        }
    }
}
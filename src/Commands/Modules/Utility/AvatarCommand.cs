using System.Threading.Tasks;
using Disqord;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule : VolteModule
    {
        [Command("Avatar")]
        [Description("Shows the mentioned user's avatar, or yours if no one is mentioned.")]
        [Remarks("avatar [@user]")]
        public Task<ActionResult> AvatarAsync(CachedMember user = null)
        {
            user ??= Context.Member;
            return Ok(Context.CreateEmbedBuilder()
                .WithAuthor(user)
                .WithImageUrl(user.GetAvatarUrl(ImageFormat.Default, 1024)));
        }
    }
}
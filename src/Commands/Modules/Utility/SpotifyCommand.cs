using System.Text;
using System.Threading.Tasks;
using Disqord;
using Humanizer;
using Qmmands;
using Volte.Commands.Results;
using Gommon;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule : VolteModule
    {
        [Command("Spotify")]
        [Description("Shows what you're listening to on Spotify, if you're listening to something.")]
        [Remarks("spotify [user]")]
        public Task<ActionResult> SpotifyAsync(CachedMember target = null)
        {
            target ??= Context.Member;
            if (target.Presence.Activity is SpotifyActivity spotify)
            {

                return Ok(Context.CreateEmbedBuilder()
                    .WithAuthor(target)
                    .WithDescription(new StringBuilder()
                        .AppendLine($"**Track:** [{spotify.TrackTitle}]({spotify.TrackUrl})")
                        .AppendLine($"**Album:** {spotify.AlbumTitle}")
                        .AppendLine(
                            $"**Duration:** {spotify.Duration.Humanize(2)}")
                        .AppendLine($"**Artist(s):** {spotify.Artists.Join(", ")}")
                        .ToString())
                    .WithThumbnailUrl(spotify.AlbumCoverUrl));
            }

            return BadRequest("Target user isn't listening to Spotify!");
        }
    }
}
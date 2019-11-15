using System.Linq;
using System.Threading.Tasks;
using Gommon;
using Humanizer;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Core.Models.Guild;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class ModerationModule : VolteModule
    {
        [Command("TagCreate", "TagAdd", "TagNew")]
        [Priority(1)]
        [Description("Creates a tag with the specified name and response.")]
        [Remarks("tagcreate {name} {response}")]
        [RequireGuildModerator]
        public Task<ActionResult> TagCreateAsync(string name, [Remainder] string response)
        {
            var d = Db.GetData(Context.Guild.Id);
            var tag = d.Extras.Tags.FirstOrDefault(t => t.Name.EqualsIgnoreCase(name));
            if (tag != null)
            {
                var user = Context.Bot.GetUserAsync(tag.CreatorId);
                return BadRequest(
                    $"Cannot make the tag **{tag.Name}**, as it already exists and is owned by **{user}**.");
            }

            tag = new Tag
            {
                Name = name,
                Response = response,
                CreatorId = Context.User.Id,
                GuildId = Context.Guild.Id,
                Uses = default
            };

            Db.ModifyData(Context.Guild, data => data.Extras.Tags.Add(tag));

            return Ok(Context.CreateEmbedBuilder()
                .WithTitle("Tag Created!")
                .AddField("Name", tag.Name)
                .AddField("Response", tag.Response)
                .AddField("Creator", Context.User.Mention));
        }

        [Command("TagDelete", "TagDel", "TagRem")]
        [Priority(1)]
        [Description("Deletes a tag if it exists.")]
        [Remarks("tagdelete {name}")]
        [RequireGuildModerator]
        public async Task<ActionResult> TagDeleteAsync([Remainder]Tag tag)
        {
            Db.ModifyData(Context.Guild, data => data.Extras.Tags.Remove(tag));
            return Ok($"Deleted the tag **{tag.Name}**, created by " +
                      $"**{await Context.Bot.GetUserAsync(tag.CreatorId)}**, with " +
                      $"**{"use".ToQuantity(tag.Uses)}**.");
        }
    }
}
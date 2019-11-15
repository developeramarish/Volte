using System.Linq;
using System.Threading.Tasks;
using Gommon;
using Qmmands;
using Volte.Commands.Results;
using Volte.Core.Models.Guild;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule : VolteModule
    {
        [Command("Tag")]
        [Priority(0)]
        [Description("Gets a tag's contents if it exists.")]
        [Remarks("tag {name}")]
        public Task<ActionResult> TagAsync([Remainder] Tag tag)
        {
            tag.Uses += 1;
            Db.ModifyData(Context.Guild.Id, x =>
                {
                    x.Extras.Tags.First(t => t.Name == tag.Name && t.Response.EqualsIgnoreCase(tag.Response)).Uses += 1;
                });

            return Ok(tag.FormatContent(Context), async _ =>
            {
                if (Db.GetData(Context.Guild.Id).Configuration.DeleteMessageOnTagCommandInvocation)
                {
                    await Context.Message.TryDeleteAsync();
                }
            }, false);
        }

        [Command("TagStats")]
        [Priority(1)]
        [Description("Shows stats for a tag.")]
        [Remarks("tagstats {name}")]
        public async Task<ActionResult> TagStatsAsync([Remainder] Tag tag)
        {
            var u = await Context.Bot.GetUserAsync(tag.CreatorId);

            return Ok(Context.CreateEmbedBuilder()
                .WithTitle($"Tag {tag.Name}")
                .AddField("Response", $"`{tag.Response}`", true)
                .AddField("Creator", $"{u}", true)
                .AddField("Uses", $"**{tag.Uses}**", true));
        }

        [Command("Tags")]
        [Description("Lists all available tags in the current guild.")]
        [Remarks("tags")]
        public Task<ActionResult> TagsAsync()
        {
            var data = Db.GetData(Context.Guild.Id);
            return Ok(Context.CreateEmbedBuilder(
                data.Extras.Tags.Count == 0
                    ? "None"
                    : $"`{data.Extras.Tags.Select(x => x.Name).Join("`, `")}`"
            ).WithTitle($"Available Tags for {Context.Guild.Name}"));
        }
    }
}
using System.Threading.Tasks;
using Gommon;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminModule : VolteModule
    {
        [Command("BlacklistAdd", "BlAdd")]
        [Description("Adds a given word/phrase to the blacklist for this guild.")]
        [Remarks("blacklistadd {phrase}")]
        [RequireGuildAdmin]
        public Task<ActionResult> BlacklistAddAsync([Remainder] string phrase)
        {
            Db.ModifyData(Context.Guild, data => data.Configuration.Moderation.Blacklist.Add(phrase));
            return Ok($"Added **{phrase}** to the blacklist.");
        }

        [Command("BlacklistRemove", "BlRem")]
        [Description("Removes a given word/phrase from the blacklist for this guild.")]
        [Remarks("blacklistremove {phrase}")]
        [RequireGuildAdmin]
        public Task<ActionResult> BlacklistRemoveAsync([Remainder] string phrase)
        {
            var d = Db.GetData(Context.Guild.Id);
            if (d.Configuration.Moderation.Blacklist.ContainsIgnoreCase(phrase))
            {
                Db.ModifyData(Context.Guild, data => data.Configuration.Moderation.Blacklist.Remove(phrase));
                return Ok($"Removed **{phrase}** from the word blacklist.");
            }

            return BadRequest($"**{phrase}** doesn't exist in the blacklist.");
        }

        [Command("BlacklistClear", "BlCl")]
        [Description("Clears the blacklist for this guild.")]
        [Remarks("blacklistclear")]
        [RequireGuildAdmin]
        public Task<ActionResult> BlacklistClearAsync()
        {
            var count = Db.GetData(Context.Guild.Id).Configuration.Moderation.Blacklist.Count;
            Db.ModifyData(Context.Guild, data => data.Configuration.Moderation.Blacklist.Clear());
            return Ok($"Cleared the this guild's blacklist, containing **{count}** words.");
        }
    }
}
using System;
using System.Linq;
using System.Threading.Tasks;
using Disqord;
using Gommon;
using Humanizer;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Core.Models;
using Volte.Core.Models.EventArgs;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class ModerationModule : VolteModule
    {
        [Command("Purge", "clear", "clean")]
        [Description("Purges the last x messages, or the last x messages by a given user.")]
        [Remarks("purge {count} [targetAuthor]")]
        [RequireBotChannelPermission(Permission.ManageMessages)]
        [RequireGuildModerator]
        public async Task<ActionResult> PurgeAsync(int count, CachedMember targetAuthor = null)
        {
            var c = Context.Channel.Cast<CachedTextChannel>();
            //+1 to include the command invocation message, and actually delete the last x messages instead of x - 1.
            //lets you theoretically use 0 to delete only the invocation message, for testing or something.
            var messages = (await Context.Channel.GetMessagesAsync(count + 1)).ToList();
            if (!(targetAuthor is null))
                await c.DeleteMessagesAsync(messages.Where(x => x.Author.Id == targetAuthor.Id).Select(x => x.Id));
            else
                await c.DeleteMessagesAsync(messages.Select(x => x.Id));

            //-1 to show that the correct amount of messages were deleted.
            return Ok($"Successfully deleted **{"message".ToQuantity(messages.Count - 1)}**", m =>
            {
                _ = Executor.ExecuteAfterDelayAsync(TimeSpan.FromSeconds(3), async () => await m.DeleteAsync());
                return ModLogService.DoAsync(ModActionEventArgs.New
                    .WithDefaultsFromContext(Context)
                    .WithActionType(ModActionType.Purge)
                    .WithCount(count));
            });
        }
    }
}
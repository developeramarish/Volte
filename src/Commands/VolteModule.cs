using System;
using System.Threading.Tasks;
using Disqord;
using Disqord.Bot;
using Qmmands;
using Volte.Commands.Results;
using Volte.Core;
using Volte.Services;

namespace Volte.Commands
{
    public abstract class VolteModule : DiscordModuleBase<VolteContext>
    {
        public DatabaseService Db { get; set; }
        public EventService EventService { get; set; }
        public ModLogService ModLogService { get; set; }
        public CommandService CommandService { get; set; }
        public EmojiService EmojiService { get; set; }
        public LoggingService Logger { get; set; }
        public VolteBot Bot { get; set; }


        protected static ActionResult Ok(
            string text, 
            Func<IUserMessage, Task> afterCompletion = null,
            bool shouldEmbed = true) 
            => new OkResult(text, shouldEmbed, null, afterCompletion);

        protected static ActionResult Ok(
            Func<Task> logic, 
            bool awaitLogic = true) 
            => new OkResult(logic, awaitLogic);


        protected static ActionResult Ok(
            LocalEmbedBuilder embed, 
            Func<IUserMessage, Task> afterCompletion = null) 
            => new OkResult(null, true, embed, afterCompletion);

        protected static ActionResult Ok(string text) 
            => new OkResult(text);

        protected static ActionResult Ok(LocalEmbedBuilder embed) 
            => new OkResult(null, true, embed);

        protected static ActionResult BadRequest(string reason) 
            => new BadRequestResult(reason);

        protected static ActionResult None(
            Func<Task> afterCompletion = null, 
            bool awaitCallback = true) 
            => new NoResult(afterCompletion, awaitCallback);
    }
}
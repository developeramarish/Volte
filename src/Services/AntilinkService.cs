using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Disqord;
using Disqord.Events;
using Gommon;
using Volte.Commands;
using Volte.Core;
using Volte.Core.Models;
using Volte.Core.Models.EventArgs;

namespace Volte.Services
{
    public sealed class AntilinkService : VolteEventService
    {
        private readonly Regex _invitePattern =
            new Regex(@"discord(?:\.gg|\.io|\.me|app\.com\/invite)\/([\w\-]+)", RegexOptions.Compiled);

        private readonly LoggingService _logger;
        private readonly VolteBot _bot;

        public AntilinkService(LoggingService loggingService,
            VolteBot bot)
        {
            _logger = loggingService;
            _bot = bot;
        } 

        public override Task DoAsync(EventArgs args) 
            => CheckMessageAsync(args.Cast<MessageReceivedEventArgs>());


        private async Task CheckMessageAsync(MessageReceivedEventArgs args)
        {
            var ctx = VolteContext.FromMessageReceivedEventArgs(args);
            if (!ctx.Db.GetData(ctx.Guild.Id).Configuration.Moderation.Antilink ||
                ctx.Member.IsAdmin(_bot)) return;

            _logger.Debug(LogSource.Volte,
                $"Checking a message in #{ctx.Channel.Name} ({ctx.Guild.Name}) for Discord invite URLs.");

            var matches = _invitePattern.Matches(args.Message.Content);
            if (!matches.Any())
            {
                _logger.Debug(LogSource.Volte,
                    $"Message checked in #{ctx.Channel.Name} ({ctx.Guild.Name}) did not contain any detectable invites; aborted.");
                return;
            }

            await args.Message.DeleteAsync(RestRequestOptions.FromReason("Deleted as it contained an invite link."));
            var m = await ctx.CreateEmbed("Don't send invites here.").SendToAsync(ctx.Channel);
            _logger.Debug(LogSource.Volte,
                $"Deleted a message in #{ctx.Channel.Name} ({ctx.Guild.Name}) for containing a Discord invite URL.");
            _ = Executor.ExecuteAfterDelayAsync(TimeSpan.FromSeconds(3), () => m.DeleteAsync());
        }
    }
}
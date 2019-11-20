using System;
using System.Threading.Tasks;
using Disqord;
using Disqord.Events;
using Gommon;
using Volte.Core.Models;

namespace Volte.Services
{
    public sealed class PingChecksService : VolteEventService<MessageReceivedEventArgs>
    {
        private readonly LoggingService _logger;
        private readonly DatabaseService _db;
        private readonly IServiceProvider _provider;

        public PingChecksService(LoggingService loggingService,
            DatabaseService databaseService,
            IServiceProvider serviceProvider)
        {
            _logger = loggingService;
            _db = databaseService;
            _provider = serviceProvider;
        } 
            

        public override Task DoAsync(MessageReceivedEventArgs args)
            => CheckMessageAsync(args);

        private async Task CheckMessageAsync(MessageReceivedEventArgs args)
        {
            var data = _db.GetData(args.Message.Guild.Id.RawValue);
            if (data.Configuration.Moderation.MassPingChecks &&
                !args.Message.Author.Cast<CachedMember>().IsAdmin(_provider))
            {
                _logger.Debug(LogSource.Service,
                    "Received a message to check for ping threshold violations.");
                var content = args.Message.Content;
                if (content.ContainsIgnoreCase("@everyone") ||
                    content.ContainsIgnoreCase("@here") ||
                    args.Message.MentionedUsers.Count > 10)
                {
                    _ = await args.Message.TryDeleteAsync();
                    _logger.Debug(LogSource.Service,
                        "Attempted to delete a message for violating the ping threshold. If it was not deleted, I do not have proper permissions in that guild.");
                }
            }
        }
    }
}
using System;
using System.Threading.Tasks;
using Disqord.Events;
using Gommon;
using Volte.Core.Models;

namespace Volte.Services
{
    public sealed class AutoroleService : VolteEventService
    {
        private readonly LoggingService _logger;
        private readonly DatabaseService _db;

        public AutoroleService(LoggingService loggingService,
            DatabaseService databaseService)
        {
            _logger = loggingService;
            _db = databaseService;
        }

        public override Task DoAsync(EventArgs args)
            => ApplyRoleAsync(args.Cast<MemberJoinedEventArgs>());

        private async Task ApplyRoleAsync(MemberJoinedEventArgs args)
        {
            var data = _db.GetData(args.Member.Guild.Id);
            var targetRole = args.Member.Guild.GetRole(data.Configuration.Autorole);
            if (targetRole is null)
            {
                _logger.Debug(LogSource.Volte,
                    $"Guild {args.Member.Guild.Name}'s Autorole is set to an ID of a role that no longer exists; or is not set at all.");
                return;
            }

            await args.Member.GrantRoleAsync(targetRole.Id);
            _logger.Debug(LogSource.Volte,
                $"Applied role {targetRole.Name} to user {args.Member} in guild {args.Member.Guild.Name}.");
        }
    }
}
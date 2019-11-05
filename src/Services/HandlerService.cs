using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Qmmands;
using Volte.Core.Models;
using Gommon;
using Volte.Core;

namespace Volte.Services
{
    internal sealed class HandlerService //no attr since it's manually added
    {
        private readonly VolteBot _bot;
        private readonly LoggingService _logger;

        public HandlerService(VolteBot bot,
            LoggingService loggingService)
        {
            _bot = bot;
            _logger = loggingService;
        }

        public async Task InitializeAsync()
        {
            var sw = Stopwatch.StartNew();
            var l = await _bot.AddTypeParsersAsync();
            sw.Stop();
            _logger.Info(LogSource.Volte, $"Loaded TypeParsers: [{l.Select(x => x.SanitizeParserName()).Join(", ")}] in {sw.ElapsedMilliseconds}ms.");
            sw = Stopwatch.StartNew();

            var loaded = _bot.AddModules(GetType().Assembly);
            sw.Stop();
            _logger.Info(LogSource.Volte,
                $"Loaded {loaded.Count} modules and {loaded.Sum(m => m.Commands.Count)} commands loaded in {sw.ElapsedMilliseconds}ms.");
            await _bot.RegisterVolteEventHandlersAsync();
        }
    }
}
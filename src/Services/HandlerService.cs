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
        private readonly CommandService _service;
        private readonly LoggingService _logger;

        public HandlerService(VolteBot bot,
            CommandService commandService,
            LoggingService loggingService)
        {
            _bot = bot;
            _service = commandService;
            _logger = loggingService;
        }

        public async Task InitializeAsync(VolteBot bot)
        {
            var sw = Stopwatch.StartNew();
            var l = await _service.AddTypeParsersAsync();
            sw.Stop();
            _logger.Info(LogSource.Volte, $"Loaded TypeParsers: [{l.Select(x => x.SanitizeParserName()).Join(", ")}] in {sw.ElapsedMilliseconds}ms.");
            sw = Stopwatch.StartNew();

            var loaded = _service.AddModules(GetType().Assembly);
            sw.Stop();
            _logger.Info(LogSource.Volte,
                $"Loaded {loaded.Count} modules and {loaded.Sum(m => m.Commands.Count)} commands loaded in {sw.ElapsedMilliseconds}ms.");
            await _bot.RegisterVolteEventHandlersAsync();
        }
    }
}
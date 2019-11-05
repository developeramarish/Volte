using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Disqord;
using Disqord.Bot;
using Gommon;
using Microsoft.Extensions.DependencyInjection;
using Volte.Commands;
using Volte.Core.Models;
using Volte.Services;
using Color = System.Drawing.Color;
using Console = Colorful.Console;

namespace Volte.Core
{
    public class VolteBot : DiscordBot
    {
        public static async Task StartAsync()
        {
            Console.Title = "Volte";
            Console.CursorVisible = false;

            if (!Directory.Exists(Config.DataDirectory))
            {
                Console.WriteLine($"The \"{Config.DataDirectory}\" directory didn't exist, so I created it for you.", Color.Red);
                Directory.CreateDirectory(Config.DataDirectory);
                //99.9999999999% of the time the config also won't exist if this block is reached
                //if the config does exist when this block is reached, feel free to become the lead developer of this project
            }

            if (!Config.CreateIfNonexistent())
            {
                Console.WriteLine($"Please fill in the configuration located at \"{Config.ConfigFilePath}\"; restart me when you've done so.", Color.Crimson);
                return;
            }

            Config.Load();

            if (!Config.IsValidToken()) return;

            await new VolteBot().LoginAsync();
        }

        private IServiceProvider _provider;
        private CancellationTokenSource _cts;
        private DiscordBotConfiguration _config;

        private static IServiceProvider BuildServiceProvider(VolteBot bot)
            => new ServiceCollection() 
                .AddAllServices(bot)
                .BuildServiceProvider();

        private VolteBot() : base(TokenType.Bot, Config.Token)
            => Console.CancelKeyPress += (s, _) => _cts.Cancel();

        private async Task LoginAsync()
        {

            _config = new DiscordBotConfiguration
            {
                ProviderFactory = x => BuildServiceProvider(this)
            };

            _provider = _config.ProviderFactory(this);
            

            _provider.Get(out _cts);
            _provider.Get<HandlerService>(out var handler);
            _provider.Get<LoggingService>(out var logger);

            

            await handler.InitializeAsync(this);

            try
            {
                await RunAsync(_cts.Token);
            }
            catch (TaskCanceledException) //this exception always occurs when CancellationTokenSource#Cancel() is called; so we put the shutdown logic inside the catch block
            {
                logger.Critical(LogSource.Volte,
                    "Bot shutdown requested; shutting down and cleaning up.");
                await ShutdownAsync(this, _cts);
            }
        }

        // ReSharper disable SuggestBaseTypeForParameter
        private async Task ShutdownAsync(VolteBot bot, CancellationTokenSource cts)
        {
            if (Config.GuildLogging.EnsureValidConfiguration(bot, out var channel))
            {
                await new LocalEmbedBuilder()
                    .WithErrorColor()
                    .WithAuthor(bot.GetOwner())
                    .WithDescription(
                        $"Volte {Version.FullVersion} is shutting down at **{DateTimeOffset.UtcNow.FormatFullTime()}, on {DateTimeOffset.UtcNow.FormatDate()}**. I was online for **{Process.GetCurrentProcess().GetUptime()}**!")
                    .SendToAsync(channel);
            }

            await DisposeAsync();
            await base.DisposeAsync();
            Dispose(cts, bot, DatabaseService.Database);
            Environment.Exit(0);
        }

        private void Dispose(params IDisposable[] disposables)
        {
            foreach (var disposable in disposables)
                disposable.Dispose();

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);
        }

        protected override ValueTask<bool> BeforeExecutedAsync(CachedUserMessage message) 
            => new ValueTask<bool>(message.Guild != null);

        protected override ValueTask<DiscordCommandContext> GetCommandContextAsync(CachedUserMessage message, string _) 
            => VolteContext.Create(this, string.Empty, message);

        public override object GetService(Type serviceType) 
            => serviceType == typeof(VolteBot) || serviceType == GetType()
                ? this
                : _provider.GetService(serviceType);
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Disqord;
using Disqord.Bot;
using Gommon;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using Volte.Core.Models;
using Volte.Services;
using Color = System.Drawing.Color;
using Console = Colorful.Console;

namespace Volte.Core
{
    public class VolteBot
    {
        public static Task StartAsync()
            => new VolteBot().LoginAsync();

        private DiscordBot _bot;
        private IServiceProvider _provider;
        private CancellationTokenSource _cts;

        private static IServiceProvider BuildServiceProvider()
            => new ServiceCollection() 
                .AddAllServices()
                .BuildServiceProvider();

        private VolteBot() 
            => Console.CancelKeyPress += (s, _) => _cts.Cancel();

        private async Task LoginAsync()
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

            _provider = BuildServiceProvider();

            _bot = new DiscordBot(TokenType.Bot, Config.Token, new DiscordBotConfiguration
            {
                ProviderFactory = x => _provider,
                CommandService = _provider.GetRequiredService<CommandService>()
            });

            _provider.Get(out _cts);
            _provider.Get<HandlerService>(out var handler);
            _provider.Get<LoggingService>(out var logger);

            await handler.InitializeAsync(_bot);

            try
            {
                await _bot.RunAsync(_cts.Token);
            }
            catch (TaskCanceledException) //this exception always occurs when CancellationTokenSource#Cancel() is called; so we put the shutdown logic inside the catch block
            {
                logger.Critical(LogSource.Volte,
                    "Bot shutdown requested; shutting down and cleaning up.");
                await ShutdownAsync(_bot, _cts);
            }
        }

        // ReSharper disable SuggestBaseTypeForParameter
        private async Task ShutdownAsync(DiscordBot client, CancellationTokenSource cts)
        {
            if (Config.GuildLogging.EnsureValidConfiguration(client, out var channel))
            {
                await new LocalEmbedBuilder()
                    .WithErrorColor()
                    .WithAuthor()
                    .WithDescription(
                        $"Volte {Version.FullVersion} is shutting down at **{DateTimeOffset.UtcNow.FormatFullTime()}, on {DateTimeOffset.UtcNow.FormatDate()}**. I was online for **{Process.GetCurrentProcess().GetUptime()}**!")
                    .SendToAsync(channel);
            }
            
            await client.SetStatusAsync(UserStatus.Invisible);
            await client.LogoutAsync();
            await client.StopAsync();
            Dispose(cts, client, DatabaseService.Database);
            Environment.Exit(0);
        }

        private void Dispose(params IDisposable[] disposables)
        {
            foreach (var disposable in disposables)
                disposable.Dispose();

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);
        }
    }
}
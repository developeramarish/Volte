using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Disqord;
using Disqord.Bot;
using Gommon;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using Volte.Commands;
using Volte.Core.Models;
using Volte.Core.Models.EventArgs;
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

            _config = new DiscordBotConfiguration
            {
                ProviderFactory = x => BuildServiceProvider(x.Cast<VolteBot>()),
                HasMentionPrefix = true,
                Prefixes = new[] { Config.CommandPrefix }
            };

            await new VolteBot(TokenType.Bot, Config.Token, _config).InitializeAsync();
        }

        private CancellationTokenSource _cts;
        private static DiscordBotConfiguration _config;
        private IServiceProvider _provider;

        private static IServiceProvider BuildServiceProvider(VolteBot bot)
            => new ServiceCollection() 
                .AddAllServices(bot)
                .BuildServiceProvider();

        private VolteBot(TokenType type, string token, DiscordBotConfiguration config) : base(type, token, config)
            => Console.CancelKeyPress += (s, _) => _cts.Cancel();

        private async Task InitializeAsync()
        {
            _provider = _config.ProviderFactory(this);
            this.Get(out _cts);
            this.Get<HandlerService>(out var handler);
            this.Get<LoggingService>(out var logger);

            

            await handler.InitializeAsync();

            try
            {
                await RunAsync(_cts.Token);
            }
            catch (TaskCanceledException) //this exception always occurs when CancellationTokenSource#Cancel() is called; so we put the shutdown logic inside the catch block
            {
                logger.Critical(LogSource.Volte,
                    "Bot shutdown requested; shutting down and cleaning up.");
                await ShutdownAsync();
            }
        }

        // ReSharper disable SuggestBaseTypeForParameter
        private async Task ShutdownAsync()
        {
            if (Config.GuildLogging.EnsureValidConfiguration(this, out var channel))
            {
                await new LocalEmbedBuilder()
                    .WithErrorColor()
                    .WithAuthor(this.GetOwner())
                    .WithDescription(
                        $"Volte {Version.FullVersion} is shutting down at **{DateTimeOffset.UtcNow.FormatFullTime()}, on {DateTimeOffset.UtcNow.FormatDate()}**. I was online for **{Process.GetCurrentProcess().GetUptime()}**!")
                    .SendToAsync(channel);
            }

            await base.DisposeAsync();
            Environment.Exit(0);
        }


        protected override async ValueTask<bool> BeforeExecutedAsync(CachedUserMessage message)
        {
            var r = await base.BeforeExecutedAsync(message);
            return r && message.Guild != null;
        }

        protected override async ValueTask AfterExecutedAsync(IResult result, DiscordCommandContext context)
        {
            Console.WriteLine(result.GetType());
            this.Get<CommandsService>(out var commands);
            await commands.OnCommandAsync(new CommandCalledEventArgs(result, context));
        }

        protected override ValueTask<DiscordCommandContext> GetCommandContextAsync(CachedUserMessage message, string _) 
            => VolteContext.Create(this, string.Empty, message);

        public async Task<VolteContext> GetCommandContextAsync(CachedUserMessage message)
            => (await GetCommandContextAsync(message, string.Empty)).Cast<VolteContext>();

        public override object GetService(Type serviceType) 
            => serviceType == typeof(VolteBot) || serviceType == GetType()
                ? this
                : _provider.GetService(serviceType);

        protected override ValueTask<(string Prefix, string Output)> FindPrefixAsync(CachedUserMessage message)
        {
            this.Get<DatabaseService>(out var db);
            var prefixes = new []
            {
                message.Client.CurrentUser.Mention, 
                db.GetData(message.Guild.Id).Configuration.CommandPrefix
            };
            return CommandUtilities.HasAnyPrefix(message.Content, prefixes, out var p, out var o)
                ? new ValueTask<(string Prefix, string Output)>((p, o))
                : new ValueTask<(string Prefix, string Output)>(default((string, string)));
        }
    }
}
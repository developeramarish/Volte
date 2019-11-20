﻿using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Colorful;
using Disqord;
using Disqord.Logging;
using Gommon;
using Volte.Core;
using Volte.Core.Models;
using Volte.Core.Models.EventArgs;
using Color = System.Drawing.Color;
using Console = Colorful.Console;

namespace Volte.Services
{
    public sealed class LoggingService : VolteEventService<LogEventArgs>
    {
        private readonly VolteBot _bot;
        private readonly HttpClient _http;
        private readonly object _lock;
        private const string LogFile = "data/Volte.log";

        public LoggingService(VolteBot bot,
            HttpClient httpClient)
        {
            _bot = bot;
            _http = httpClient;
            _lock = new object();
        }

        public override Task DoAsync(LogEventArgs args)
        {
            Log(args);
            return Task.CompletedTask;
        }

        private void Log(LogEventArgs args) =>
            Log(args.LogMessage.Internal.Severity, args.LogMessage.Internal.Source,
                args.LogMessage.Internal.Message, args.LogMessage.Internal.Exception);

        internal void PrintVersion()
        {
            Info(LogSource.Volte, "--------------------------------------------");
            foreach (var asciiLine in new Figlet().ToAscii("VOLTE").ConcreteValue.Split("\n")) //i had to look at colorful.console's source for this snippet lol
            {
                Info(LogSource.Volte, asciiLine);
            }
            Info(LogSource.Volte, "--------------------------------------------");
            Info(LogSource.Volte, $"Currently running Volte V{Version.FullVersion}.");
        }

        private void Log(LogMessageSeverity s, LogSource from, string message, Exception e = null)
        {
            lock (_lock)
            {
                if (s is LogMessageSeverity.Debug)
                {
                    if (from is LogSource.Volte && !Config.EnableDebugLogging) return;
                }

                Execute(s, from, message, e);
            }
        }

        /// <summary>
        ///     Prints a <see cref="LogMessageSeverity.Debug"/> message to the console from the specified <paramref name="src"/> source, with the given <paramref name="message"/> message.
        /// </summary>
        /// <param name="src">Source to print the message from.</param>
        /// <param name="message">Message to print.</param>
        public void Debug(LogSource src, string message) 
            => Log(LogMessageSeverity.Debug, src, message);

        /// <summary>
        ///     Prints a <see cref="LogMessageSeverity.Information"/> message to the console from the specified <paramref name="src"/> source, with the given <paramref name="message"/> message.
        /// </summary>
        /// <param name="src">Source to print the message from.</param>
        /// <param name="message">Message to print.</param>
        public void Info(LogSource src, string message)
            => Log(LogMessageSeverity.Information, src, message);

        /// <summary>
        ///     Prints a <see cref="LogMessageSeverity.Error"/> message to the console from the specified <paramref name="src"/> source, with the given <paramref name="message"/> message, with the specified <paramref name="e"/> exception if provided.
        /// </summary>
        /// <param name="src">Source to print the message from.</param>
        /// <param name="message">Message to print.</param>
        /// <param name="e">Optional Exception to print.</param>
        public void Error(LogSource src, string message, Exception e = null)
            => Log(LogMessageSeverity.Error, src, message, e);
        /// <summary>
        ///     Prints a <see cref="LogMessageSeverity.Critical"/> message to the console from the specified <paramref name="src"/> source, with the given <paramref name="message"/> message, with the specified <paramref name="e"/> exception if provided.
        /// </summary>
        /// <param name="src">Source to print the message from.</param>
        /// <param name="message">Message to print.</param>
        /// <param name="e">Optional Exception to print.</param>
        public void Critical(LogSource src, string message, Exception e = null)
            => Log(LogMessageSeverity.Critical, src, message, e);

        /// <summary>
        ///     Prints a <see cref="LogMessageSeverity.Critical"/> message to the console from the specified <paramref name="src"/> source, with the given <paramref name="message"/> message, with the specified <paramref name="e"/> exception if provided.
        /// </summary>
        /// <param name="src">Source to print the message from.</param>
        /// <param name="message">Message to print.</param>
        /// <param name="e">Optional Exception to print.</param>
        public void Warn(LogSource src, string message, Exception e = null)
            => Log(LogMessageSeverity.Warning, src, message, e);
        /// <summary>
        ///     Prints a <see cref="LogMessageSeverity.Verbose"/> message to the console from the specified <paramref name="src"/> source, with the given <paramref name="message"/> message.
        /// </summary>
        /// <param name="src">Source to print the message from.</param>
        /// <param name="message">Message to print.</param>
        public void Trace(LogSource src, string message)
            => Log(LogMessageSeverity.Trace, src, message);

        /// <summary>
        ///     Prints a <see cref="LogMessageSeverity.Error"/> message to the console from the specified <paramref name="e"/> exception.
        /// </summary>
        /// <param name="e">Exception to print.</param>
        public void Exception(Exception e)
            => Execute(LogMessageSeverity.Error, LogSource.Volte, string.Empty, e);

        private void Execute(LogMessageSeverity s, LogSource src, string message, Exception e)
        {
            var content = new StringBuilder();
            var (color, value) = VerifySeverity(s);
            Append($"{value}:".PadRight(10), color);
            var dto = DateTimeOffset.UtcNow;
            content.Append($"[{dto.FormatDate()} | {dto.FormatFullTime()}] {value} -> ");

            (color, value) = VerifySource(src);
            Append($"[{value}]".PadRight(10), color);
            content.Append($"{value} -> ");

            if (!message.IsNullOrWhitespace())
            {
                Append(message, Color.White);
                content.Append(message);
            }

            if (e != null)
            {
                var toWrite = $"{Environment.NewLine}{e.Message}{Environment.NewLine}{e.StackTrace}";
                Append(toWrite, Color.IndianRed);
                content.Append(toWrite);
                LogExceptionInDiscord(e);
            }

            Console.Write(Environment.NewLine);
            content.AppendLine();
            if (Config.EnabledFeatures.LogToFile)
            {
                File.AppendAllText(LogFile, content.ToString());
            }
        }

        private void Append(string m, Color c)
        {
            Console.ForegroundColor = c;
            Console.Write(m);
        }

        private (Color Color, string Source) VerifySource(LogSource source) =>
            source switch
                {
                LogSource.Discord => (Color.RoyalBlue, "DISCORD"),
                LogSource.Gateway => (Color.RoyalBlue, "DISCORD"),
                LogSource.Volte => (Color.LawnGreen, "CORE"),
                LogSource.Service => (Color.Gold, "SERVICE"),
                LogSource.Module => (Color.LimeGreen, "MODULE"),
                LogSource.Rest => (Color.Red, "REST"),
                LogSource.Unknown => (Color.Fuchsia, "UNKNOWN"),
                _ => throw new InvalidOperationException($"The specified LogSource {source} is invalid.")
                };


        private (Color Color, string Level) VerifySeverity(LogMessageSeverity severity) =>


            severity switch
            {
                LogMessageSeverity.Critical => (Color.Maroon, "CRITICAL"),
                LogMessageSeverity.Error => (Color.DarkRed, "ERROR"),
                LogMessageSeverity.Warning => (Color.Yellow, "WARN"),
                LogMessageSeverity.Information => (Color.SpringGreen, "INFO"),
                LogMessageSeverity.Trace => (Color.Pink, "TRACE"),
                LogMessageSeverity.Debug => (Color.SandyBrown, "DEBUG"),
                _ => throw new InvalidOperationException($"The specified LogMessageSeverity ({severity}) is invalid.")
    };

        private void LogExceptionInDiscord(Exception e)
        {
            if (!Config.GuildLogging.EnsureValidConfiguration(_bot, out var channel))
            {
                Error(LogSource.Volte, "Invalid guild_logging.guild_id/guild_logging.channel_id configuration. Check your IDs and try again.");
                return;
            }

            _ = Task.Run(async () =>
            {
                var response = await _http.PostAsync("https://paste.greemdev.net/documents", new StringContent(e.StackTrace, Encoding.UTF8, "text/plain"));                
                var jDocument = JsonDocument.Parse(await response.Content.ReadAsStringAsync());                
                var url = $"https://paste.greemdev.net/{jDocument.RootElement.GetProperty("key").GetString()}.cs";
                await new LocalEmbedBuilder()
                    .WithErrorColor()
                    .WithTitle($"Exception at {DateTimeOffset.UtcNow.FormatDate()}, {DateTimeOffset.UtcNow.FormatFullTime()} UTC")
                    .AddField("Exception Type", e.GetType(), true)
                    .AddField("Exception Message", e.Message, true)
                    .WithDescription($"View the full Stack Trace [here]({url}).")
                    .SendToAsync(channel);
            });
        }
    }
}
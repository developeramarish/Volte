using System;
using Disqord.Logging;

namespace Volte.Core.Models
{
    public sealed class LogMessage
    {
        public LogMessageSeverity Severity { get; private set; }
        public LogSource Source { get; private set; }
        public string Message { get; private set; }
        public Exception Exception { get; private set; }

        public static LogMessage FromDiscordLogMessage(MessageLoggedEventArgs message)
            => new LogMessage
            {
                Message = message.Message,
                Severity = message.Severity,
                Exception = message.Exception,
                Source = message.Source switch
                {
                    "Rest" => LogSource.Rest,
                    "Discord" => LogSource.Discord,
                    "Gateway" => LogSource.Gateway,
                    _ => LogSource.Unknown
                }
            };
    }
}
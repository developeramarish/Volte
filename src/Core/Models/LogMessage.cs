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

        public static LogMessage FromDisqordLogMessage(MessageLoggedEventArgs args)
            => new LogMessage
            {
                Message = args.Message,
                Severity = args.Severity,
                Exception = args.Exception,
                Source = args.Source switch
                {
                    "Rest" => LogSource.Rest,
                    "Discord" => LogSource.Discord,
                    "Gateway" => LogSource.Gateway,
                    "Client" => LogSource.Volte,
                    "Bot" => LogSource.Volte,
                    _ => LogSource.Unknown
                }
            };
    }
}
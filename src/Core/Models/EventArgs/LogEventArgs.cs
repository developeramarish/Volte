using Disqord.Logging;

namespace Volte.Core.Models.EventArgs
{
    public sealed class LogEventArgs : System.EventArgs
    {
        public string Message { get; }
        public string Source { get; }
        public LogMessageSeverity Severity { get; }
        public (LogMessage Internal,  MessageLoggedEventArgs External) LogMessage { get; }

        public LogEventArgs(MessageLoggedEventArgs args)
        {
            Message = args.Message;
            Source = args.Source;
            Severity = args.Severity;
            LogMessage = (Models.LogMessage.FromDisqordLogMessage(args), args);
        }
    }
}
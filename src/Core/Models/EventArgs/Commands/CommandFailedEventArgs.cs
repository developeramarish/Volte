using System.Diagnostics;
using Qmmands;
using Volte.Commands;

namespace Volte.Core.Models.EventArgs
{
    public sealed class CommandFailedEventArgs : CommandEventArgs
    {
        public FailedResult Result { get; }
        public override VolteContext Context { get; }
        public string Arguments { get; }

        public CommandFailedEventArgs(FailedResult res, VolteContext ctx, string args)
        {
            Result = res;
            Context = ctx;
            Arguments = args;
        }
    }
}

using System;
using System.Threading.Tasks;

namespace Volte.Services
{
    public abstract class VolteEventService<TArgs> : VolteService where TArgs : EventArgs
    {
        public abstract Task DoAsync(TArgs args);
    }
}

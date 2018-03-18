using System;
using System.Collections.Generic;
using System.Text;
using CurrencyTrackerServer.Infrastructure.Entities;

namespace CurrencyTrackerServer.Infrastructure.Abstract.Workers
{
    public interface IWorker<T>
    {
        UpdateSource Source { get; }
        event EventHandler<T> Updated;
    }
}

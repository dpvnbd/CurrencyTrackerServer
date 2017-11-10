using System;
using CurrencyTrackerServer.Infrastructure.Entities;

namespace CurrencyTrackerServer.Infrastructure.Abstract.Workers
{
    public interface IMonitor<T>:IDisposable
    {
        UpdateSource Source { get; }
        UpdateDestination Destination { get; }
        string UserId { get; }
        event EventHandler<T> Changed;
    }
}
using System.Collections.Generic;
using CurrencyTrackerServer.ChangeTrackerService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Entities.Changes;

namespace CurrencyTrackerServer.ChangeTrackerService.Concrete.ProviderSpecific.Poloniex
{
    public class PoloniexTimerWorker:ChangeTimerWorker
    {
        public PoloniexTimerWorker(PoloniexChangeMonitor monitor, INotifier<Change> notifier, int period = 10000) : base(monitor, notifier, period)
        {
        }
    }
}

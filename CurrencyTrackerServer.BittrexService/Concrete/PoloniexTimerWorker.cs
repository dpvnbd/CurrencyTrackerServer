using System;
using System.Collections.Generic;
using System.Text;
using CurrencyTrackerServer.ChangeTrackerService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract;

namespace CurrencyTrackerServer.ChangeTrackerService.Concrete
{
    public class PoloniexTimerWorker:ChangeTimerWorker
    {
        public PoloniexTimerWorker(INotifier<Change> notifier, int period = 10000) : base(
            new ChangeMonitor<Repository<CurrencyStateEntity>, Repository<ChangeHistoryEntryEntity>>(
                new PoloniexApiDataSource()), notifier, period)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Abstract.Data;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.NoticesService.Abstract;

namespace CurrencyTrackerServer.NoticesService.Concrete
{
    public class PoloniexNoticesMonitor:NoticesMonitor
    {
        public PoloniexNoticesMonitor(IRepositoryFactory repoFactory, DefaultNoticesTimerWorker defaultNotices,
            INotifier notifier)
            : base(repoFactory, notifier)
        {
            defaultNotices.Updated += CheckNotices;
        }

        public override UpdateSource Source => UpdateSource.Poloniex;
    }
}

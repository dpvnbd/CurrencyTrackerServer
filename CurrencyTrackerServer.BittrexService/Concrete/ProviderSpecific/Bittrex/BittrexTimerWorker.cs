using CurrencyTrackerServer.ChangeTrackerService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Entities.Changes;

namespace CurrencyTrackerServer.ChangeTrackerService.Concrete.ProviderSpecific.Bittrex
{
   public class BittrexTimerWorker : ChangeTimerWorker
    {
        public BittrexTimerWorker(BittrexChangeMonitor monitor, INotifier<Change> notifier, int period = 10000) : base(monitor, notifier, period)
        {
            ChangeSource = ChangeSource.Bittrex;
        }
    }
}
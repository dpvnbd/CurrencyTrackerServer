using CurrencyTrackerServer.ChangeTrackerService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract;

namespace CurrencyTrackerServer.ChangeTrackerService.Concrete.ProviderSpecific.Bittrex
{
   public class BittrexTimerWorker : ChangeTimerWorker
    {
        public BittrexTimerWorker(BittrexChangeMonitor monitor, INotifier<Change> notifier, int period = 10000) : base(monitor, notifier, period)
        {
        }
    }
}
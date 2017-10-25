using CurrencyTrackerServer.ChangeTrackerService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Entities.Changes;

namespace CurrencyTrackerServer.ChangeTrackerService.Concrete.ProviderSpecific.Bittrex
{
   public class BittrexTimerWorker : ChangeTimerWorker
    {
        public BittrexTimerWorker(BittrexChangeMonitor monitor, INotifier<Change> notifier) : base(monitor, notifier)
        {
        }
    }
}
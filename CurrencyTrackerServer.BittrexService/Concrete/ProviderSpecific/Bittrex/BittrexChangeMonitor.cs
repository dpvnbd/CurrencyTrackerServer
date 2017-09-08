using CurrencyTrackerServer.ChangeTrackerService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract;

namespace CurrencyTrackerServer.ChangeTrackerService.Concrete.ProviderSpecific.Bittrex
{
    public class BittrexChangeMonitor:ChangeMonitor
    {
        public BittrexChangeMonitor(IRepository<CurrencyStateEntity> stateRepo, IRepository<ChangeHistoryEntryEntity> historyRepo) : base(new BittrexApiDataSource(), stateRepo, historyRepo)
        {
        }
    }
}

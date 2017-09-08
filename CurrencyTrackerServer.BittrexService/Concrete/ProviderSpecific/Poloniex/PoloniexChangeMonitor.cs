using CurrencyTrackerServer.ChangeTrackerService.Concrete.ProviderSpecific.Bittrex;
using CurrencyTrackerServer.ChangeTrackerService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract;

namespace CurrencyTrackerServer.ChangeTrackerService.Concrete.ProviderSpecific.Poloniex
{
    public class PoloniexChangeMonitor:ChangeMonitor
    {
        public PoloniexChangeMonitor(IRepository<CurrencyStateEntity> stateRepo, IRepository<ChangeHistoryEntryEntity> historyRepo) : base(new PoloniexApiDataSource(), stateRepo, historyRepo)
        {
        }
    }
}

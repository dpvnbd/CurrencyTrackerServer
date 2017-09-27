using System.Collections.Generic;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.Data;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.ProviderSpecific.Bittrex;
using CurrencyTrackerServer.ChangeTrackerService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract;

namespace CurrencyTrackerServer.ChangeTrackerService.Concrete.ProviderSpecific.Poloniex
{
    public class PoloniexChangeMonitor:ChangeMonitor
    {
        public PoloniexChangeMonitor(IRepositoryFactory repoFactory, IChangeSettingsProvider settingsProvider) 
            : base(new PoloniexApiDataSource(), repoFactory, settingsProvider)
        {
        }
    }
}

using System.Collections.Generic;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.Data;
using CurrencyTrackerServer.ChangeTrackerService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Entities.Changes;

namespace CurrencyTrackerServer.ChangeTrackerService.Concrete.ProviderSpecific.Bittrex
{
    public class BittrexChangeMonitor:ChangeMonitor
    {
        public BittrexChangeMonitor(IRepositoryFactory repoFactory, ISettingsProvider<ChangeSettings> settingsProvider) : base(new BittrexApiDataSource(), repoFactory, settingsProvider)
        {
        }
    }
}

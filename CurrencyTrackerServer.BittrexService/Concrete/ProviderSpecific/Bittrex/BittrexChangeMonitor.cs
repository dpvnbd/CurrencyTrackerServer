using System.Collections.Generic;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.Data;
using CurrencyTrackerServer.ChangeTrackerService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract;

namespace CurrencyTrackerServer.ChangeTrackerService.Concrete.ProviderSpecific.Bittrex
{
    public class BittrexChangeMonitor:ChangeMonitor
    {
        public BittrexChangeMonitor(RepositoryFactory repoFactory) : base(new BittrexApiDataSource(), repoFactory)
        {
        }
    }
}

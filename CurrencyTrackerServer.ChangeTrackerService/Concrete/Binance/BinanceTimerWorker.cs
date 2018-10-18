using System.Collections.Generic;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.Poloniex;
using CurrencyTrackerServer.ChangeTrackerService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Abstract.Changes;
using CurrencyTrackerServer.Infrastructure.Abstract.Data;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Changes;
using Microsoft.Extensions.Options;

namespace CurrencyTrackerServer.ChangeTrackerService.Concrete.Binance
{
   public class BinanceTimerWorker : ChangeTimerWorker
    {
      public BinanceTimerWorker(BinanceApiDataSource dataSource, INotifier notifier,
        ISettingsProvider settingsProvider, IRepositoryFactory repoFactory, IOptions<AppSettings> config,
        IChangesStatsService<CurrencyChangeApiData> stats) 
      : base(dataSource, notifier, settingsProvider, repoFactory, config, stats)
      {
      }

      public override UpdateSource Source => UpdateSource.Binance;
    }
}
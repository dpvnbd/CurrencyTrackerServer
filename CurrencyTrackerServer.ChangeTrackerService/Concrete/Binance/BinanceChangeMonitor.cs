using System.Collections.Generic;
using CurrencyTrackerServer.ChangeTrackerService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract.Data;
using CurrencyTrackerServer.Infrastructure.Abstract.Workers;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Changes;
using Microsoft.Extensions.Options;

namespace CurrencyTrackerServer.ChangeTrackerService.Concrete.Binance
{
  public class BinanceChangeMonitor : ChangeMonitor
  {
    public BinanceChangeMonitor(BinanceTimerWorker changeWorker, IRepositoryFactory repoFactory,
        ISettingsProvider settingsProvider, IOptions<AppSettings> config, string userId)
  : base(changeWorker, repoFactory, settingsProvider, config, userId)
    {
    }

    public override UpdateSource Source => UpdateSource.Binance;
  }
}

using System;
using System.Collections.Generic;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.Bittrex;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.Poloniex;
using CurrencyTrackerServer.Infrastructure.Abstract.Workers;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Price;
using CurrencyTrackerServer.PriceService.Concrete.Bittrex;
using CurrencyTrackerServer.PriceService.Concrete.Poloniex;

namespace CurrencyTrackerServer.Web.Infrastructure.Concrete.MultipleUsers
{
  public sealed class UserMonitorsContainer : AbstractUserMonitorsContainer
  {
    public PoloniexPriceMonitor PoloniexPriceMonitor { get; }
    public BittrexPriceMonitor BittrexPriceMonitor { get; }
    public PoloniexChangeMonitor PoloniexChangeMonitor { get; }
    public BittrexChangeMonitor BittrexChangeMonitor { get; }

    public override string UserToken { get; set; }
    public override IList<IMonitor<IEnumerable<BaseChangeEntity>>> Monitors { get; }

    public UserMonitorsContainer(PoloniexPriceMonitor pMonitor, BittrexPriceMonitor bMonitor,
      PoloniexChangeMonitor pChange, BittrexChangeMonitor bChange)
    {
      PoloniexPriceMonitor = pMonitor;
      BittrexPriceMonitor = bMonitor;
      PoloniexChangeMonitor = pChange;
      BittrexChangeMonitor = bChange;

      Monitors = new List<IMonitor<IEnumerable<BaseChangeEntity>>> { BittrexPriceMonitor, PoloniexPriceMonitor,
       PoloniexChangeMonitor, BittrexChangeMonitor };

      foreach (var monitor in Monitors)
      {
        monitor.Changed += MonitorOnChanged;
      }
    }

    private void MonitorOnChanged(object sender, IEnumerable<BaseChangeEntity> changes)
    {
      ChangedCallback?.Invoke(UserToken, changes);
    }
  }
}

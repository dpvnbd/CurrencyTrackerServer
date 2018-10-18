using CurrencyTrackerServer.ChangeTrackerService.Concrete.Binance;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.Bittrex;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.Poloniex;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Abstract.Data;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Changes;
using CurrencyTrackerServer.Infrastructure.Entities.Price;
using CurrencyTrackerServer.PriceService.Concrete.Bittrex;
using CurrencyTrackerServer.PriceService.Concrete.Poloniex;
using Microsoft.Extensions.Options;

namespace CurrencyTrackerServer.Web.Infrastructure.Concrete.MultipleUsers
{
  public class UserContainerFactory
  {
    private readonly BittrexTimerWorker _bChangeWorker;
    private readonly PoloniexTimerWorker _pChangeWorker;
    private readonly BinanceTimerWorker _bnChangeWorker;
    private readonly BittrexPriceTimerWorker _bPriceWorker;
    private readonly PoloniexPriceTimerWorker _pPriceWorker;
    private readonly ISettingsProvider _settingsProvider;
    private readonly IMessageNotifier _notifier;
    private readonly IRepositoryFactory _repoFactory;
    private readonly IOptions<AppSettings> _config;

    public UserContainerFactory(BittrexTimerWorker bChangeWorker, PoloniexTimerWorker pChangeWorker,
      BinanceTimerWorker bnChangeWorker, BittrexPriceTimerWorker bPriceWorker, PoloniexPriceTimerWorker pPriceWorker,
      ISettingsProvider settingsProvider, IMessageNotifier notifier,
      IRepositoryFactory repoFactory, IOptions<AppSettings> config)
    {
      _bChangeWorker = bChangeWorker;
      _pChangeWorker = pChangeWorker;
      _bnChangeWorker = bnChangeWorker;
      _bPriceWorker = bPriceWorker;
      _pPriceWorker = pPriceWorker;
      _settingsProvider = settingsProvider;
      _notifier = notifier;
      _repoFactory = repoFactory;
      _config = config;
    }

    public UserMonitorsContainer Create(string userId)
    {
      var pPrice = new PoloniexPriceMonitor(_pPriceWorker, _settingsProvider, _notifier, userId);
      var bPrice = new BittrexPriceMonitor(_bPriceWorker, _settingsProvider, _notifier, userId);

      var pChange = new PoloniexChangeMonitor(_pChangeWorker, _repoFactory, _settingsProvider, _config, userId);
      var bChange = new BittrexChangeMonitor(_bChangeWorker, _repoFactory, _settingsProvider, _config, userId);
      var bnChange = new BinanceChangeMonitor(_bnChangeWorker, _repoFactory, _settingsProvider, _config, userId);

      return new UserMonitorsContainer(pPrice, bPrice, pChange, bChange, bnChange);
    }
  }
}

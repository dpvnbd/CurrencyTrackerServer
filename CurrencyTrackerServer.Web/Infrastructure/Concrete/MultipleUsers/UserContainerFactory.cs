using CurrencyTrackerServer.ChangeTrackerService.Concrete.Bittrex;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.Poloniex;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Abstract.Data;
using CurrencyTrackerServer.Infrastructure.Entities.Changes;
using CurrencyTrackerServer.Infrastructure.Entities.Price;
using CurrencyTrackerServer.PriceService.Concrete.Bittrex;
using CurrencyTrackerServer.PriceService.Concrete.Poloniex;

namespace CurrencyTrackerServer.Web.Infrastructure.Concrete.MultipleUsers
{
  public class UserContainerFactory
  {
    private readonly BittrexTimerWorker _bChangeWorker;
    private readonly PoloniexTimerWorker _pChangeWorker;
    private readonly BittrexPriceTimerWorker _bPriceWorker;
    private readonly PoloniexPriceTimerWorker _pPriceWorker;
    private readonly ISettingsProvider _settingsProvider;
    private readonly IMessageNotifier _notifier;
    private readonly IRepositoryFactory _repoFactory;

    public UserContainerFactory(BittrexTimerWorker bChangeWorker, PoloniexTimerWorker pChangeWorker,
      BittrexPriceTimerWorker bPriceWorker, PoloniexPriceTimerWorker pPriceWorker,
      ISettingsProvider settingsProvider, IMessageNotifier notifier,
      IRepositoryFactory repoFactory)
    {
      _bChangeWorker = bChangeWorker;
      _pChangeWorker = pChangeWorker;
      _bPriceWorker = bPriceWorker;
      _pPriceWorker = pPriceWorker;
      _settingsProvider = settingsProvider;
      _notifier = notifier;
      _repoFactory = repoFactory;
    }

    public UserMonitorsContainer Create(string userId)
    {
      var pPrice = new PoloniexPriceMonitor(_pPriceWorker, _settingsProvider, _notifier, userId);
      var bPrice = new BittrexPriceMonitor(_bPriceWorker, _settingsProvider, _notifier, userId);

      var pChange = new PoloniexChangeMonitor(_pChangeWorker, _repoFactory, _settingsProvider, userId);
      var bChange = new BittrexChangeMonitor(_bChangeWorker, _repoFactory, _settingsProvider, userId);

      return new UserMonitorsContainer(pPrice, bPrice, pChange, bChange);
    }
  }
}

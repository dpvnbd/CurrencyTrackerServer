using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CurrencyTrackerServer.ChangeTrackerService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Abstract.Changes;
using CurrencyTrackerServer.Infrastructure.Abstract.Data;
using CurrencyTrackerServer.Infrastructure.Abstract.Workers;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Changes;
using CurrencyTrackerServer.Infrastructure.Entities.Data;
using Microsoft.Extensions.Options;

namespace CurrencyTrackerServer.ChangeTrackerService.Concrete
{
  public abstract class ChangeTimerWorker : AbstractTimerWorker<IEnumerable<CurrencyChangeApiData>>
  {
    private readonly IDataSource<IEnumerable<CurrencyChangeApiData>> _dataSource;
    private readonly INotifier _notifier;
    private readonly ISettingsProvider _settingsProvider;
    private readonly IRepositoryFactory _repoFactory;
    private readonly IChangesStatsService<CurrencyChangeApiData> _statsService;

    private readonly int _updateClientsCyclePeriod = 3;
    private int _currentCycle;


    public ChangeTimerWorker(IDataSource<IEnumerable<CurrencyChangeApiData>> dataSource, INotifier notifier,
            ISettingsProvider settingsProvider, IRepositoryFactory repoFactory, IOptions<AppSettings> config,
            IChangesStatsService<CurrencyChangeApiData> statsService)
    {
      _dataSource = dataSource;
      _notifier = notifier;
      _settingsProvider = settingsProvider;
      _repoFactory = repoFactory;
      _statsService = statsService;
      Period = config.Value.ChangeWorkerPeriodSeconds * 1000;
      _updateClientsCyclePeriod = config.Value.ChangeWorkerUpdateCycle;
    }



    protected override async Task DoWork()
    {
      try
      {
        var changes = await _dataSource.GetData();
        _currentCycle++;
        if (changes.Any())
        {
          OnUpdated(changes);
          await _statsService.UpdateStates(changes);
          await _statsService.ProcessAverageChange(changes);
        }
      }
      catch (Exception e)
      {
        var errorMessage = new Change
        {
          Type = UpdateType.Error,
          Message = e.Message,
          Time = DateTime.Now,
          Source = Source
        };

        await _notifier.SendToAll(new[] { errorMessage });
      }
      finally
      {
        if (_currentCycle >= _updateClientsCyclePeriod)
        {
          var update = new BaseChangeEntity
          {
            Destination = UpdateDestination.CurrencyChange,
            Source = Source,
            Type = UpdateType.Info,
            Time = DateTimeOffset.Now
          };

          await _notifier.SendToAll(new[] { update });
          _currentCycle = 0;
        }
      }
    }

  }
}
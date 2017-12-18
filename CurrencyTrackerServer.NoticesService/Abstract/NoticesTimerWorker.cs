using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Abstract.Workers;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Notices;
using Microsoft.Extensions.Options;
using Serilog;

namespace CurrencyTrackerServer.NoticesService.Abstract
{
    public abstract class NoticesTimerWorker : AbstractTimerWorker<IEnumerable<Notice>>
    {
        private readonly IEnumerable<IDataSource<IEnumerable<Notice>>> _sources;

        public NoticesTimerWorker(IEnumerable<IDataSource<IEnumerable<Notice>>> sources, IOptions<AppSettings> config)
        {
            _sources = sources;
            Period = config.Value.NoticesWorkerPeriodSeconds * 1000;
        }
    protected override void DoWork()
    {
      var notices = new List<Notice>();

      foreach (var dataSource in _sources)
      {
        try
        {
          var prices = dataSource.GetData();
          notices.AddRange(prices.Take(5));
        }
        catch (Exception e1)
        {
          Log.Debug(e1, "Notices error");
        }
      }

      if (notices.Any())
      {
        OnUpdated(notices);
      }

    }

  }
}

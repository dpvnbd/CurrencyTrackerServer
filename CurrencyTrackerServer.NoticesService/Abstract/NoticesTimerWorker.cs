using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Abstract.Workers;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Notices;
using Serilog;

namespace CurrencyTrackerServer.NoticesService.Abstract
{
    public abstract class NoticesTimerWorker:AbstractTimerWorker<IEnumerable<Notice>>
    {
        private readonly IEnumerable<IDataSource<IEnumerable<Notice>>> _sources;

        public NoticesTimerWorker(IEnumerable<IDataSource<IEnumerable<Notice>>> sources)
        {
            _sources = sources;
        }
        protected override async Task DoWork()
        {
            var notices = new List<Notice>();

            try
            {
                foreach (var dataSource in _sources)
                {
                    try
                    {
                        var prices = await dataSource.GetData();
                        notices.AddRange(prices.Take(5));
                    }
                    catch (Exception e1)
                    {
                        Log.Warning(e1, "Notices error ");
                    }
                }

                if (notices.Any())
                {
                    OnUpdated(notices);
                }
            }
            finally
            {
                //TODO - Load period from config
                Period = 60 * 1000;
            }
        }

      
    }
}

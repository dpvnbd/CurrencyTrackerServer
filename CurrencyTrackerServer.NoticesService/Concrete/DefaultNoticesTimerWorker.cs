using System;
using System.Collections.Generic;
using System.Text;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Notices;
using CurrencyTrackerServer.NoticesService.Abstract;
using Microsoft.Extensions.Options;

namespace CurrencyTrackerServer.NoticesService.Concrete
{
    public class DefaultNoticesTimerWorker : NoticesTimerWorker
    {
        public DefaultNoticesTimerWorker(PoloniexTwitterNoticesDataSource twitterNotices,
            PoloniexSiteNoticesDataSource siteNotices, IOptions<AppSettings> config)
            : base(new IDataSource<IEnumerable<Notice>>[] { twitterNotices, siteNotices }, config)
        {
            
        }

        public override UpdateSource Source => UpdateSource.Poloniex;
    }
}

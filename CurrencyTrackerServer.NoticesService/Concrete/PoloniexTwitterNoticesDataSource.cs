using System;
using System.Collections.Generic;
using System.Text;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.NoticesService.Entities;
using Microsoft.Extensions.Options;

namespace CurrencyTrackerServer.NoticesService.Concrete
{
    public class PoloniexTwitterNoticesDataSource:TwitterNoticesDataSource
    {
        public PoloniexTwitterNoticesDataSource(IOptions<TwitterSettings> settings) : base(settings)
        {
        }

        public override UpdateSource Source => UpdateSource.Poloniex;
    }
}

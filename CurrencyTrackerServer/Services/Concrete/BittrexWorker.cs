using System;
using System.Collections.Generic;
using System.Linq;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Concrete;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Services.Abstract;
using Microsoft.Extensions.Options;

namespace CurrencyTrackerServer.Services.Concrete
{
    public class BittrexWorker : TimerWorker<IEnumerable<BittrexChange>>
    {
        private readonly IBittrexService _service;
        private readonly NotificationsMessageHandler _notificationSender;
        private static object apiLock = new object();

        public BittrexWorker(IBittrexService service, NotificationsMessageHandler notificationSender, int period = 5000)
            : base(period)
        {
            _service = service;
            _notificationSender = notificationSender;
        }

        public int Percentage { get; set; } = 10;
        public bool ChangeOverTime { get; set; } = true;
        public TimeSpan ChangeInterval { get; set; } = TimeSpan.FromMinutes(2);

        protected override void DoWork()
        {
            lock (apiLock)
            {
                var start = DateTime.Now;
                IEnumerable<BittrexChange> result;

                try
                {
                    result = _service.LoadChangesGreaterThan(Percentage, ChangeOverTime, ChangeInterval);
                }
                catch (Exception e)
                {
                    _notificationSender.SendInfoMessage(e.Message);
                    return;
                }

                var bittrexChanges = result as IList<BittrexChange> ?? result.ToList();
                var end = DateTime.Now;

                if (bittrexChanges != null && bittrexChanges.Any())
                {
                    _notificationSender.SendNotificationMessage(bittrexChanges);
                    //_notificationSender.SendInfoMessage($"Запрос: {start:T} - {end:T}");
                }
            }
        }
    }
}
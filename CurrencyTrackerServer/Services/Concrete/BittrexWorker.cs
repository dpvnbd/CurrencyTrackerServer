using System;
using System.Collections.Generic;
using System.Linq;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Services.Abstract;

namespace CurrencyTrackerServer.Services.Concrete
{
    public class BittrexWorker:TimerWorker<IEnumerable<BittrexChange>>
    {
        private readonly IBittrexService _service;
        private static object apiLock = new object();
        public BittrexWorker(IBittrexService service, int period = 3000) : base(period)
        {
            _service = service;
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
                    SendMessage(e.Message);
                    return;
                }

                var bittrexChanges = result as IList<BittrexChange> ?? result.ToList();
                var end = DateTime.Now;

                if (bittrexChanges != null && bittrexChanges.Any())
                {
                    SendMessage(bittrexChanges);
                    SendMessage($"Запрос: {start:T} - {end:T}");
                }
            }
        }
    }
}

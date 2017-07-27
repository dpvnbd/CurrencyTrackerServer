using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyTrackerServer.BittrexService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract;

namespace CurrencyTrackerServer.BittrexService.Concrete
{
    public class BittrexTimerWorker : AbstractTimerWorker<List<Change>>
    {
        private readonly IChangeMonitor<List<Change>> _monitor;
        private readonly INotifier<Change> _notifier;

        public int Percentage { get; set; }
        
        public TimeSpan ResetTimeSpan { get; set; }

        public bool MultipleChanges { get; set; }

        public TimeSpan MultipleChangesSpan { get; set; }

        public BittrexTimerWorker(IChangeMonitor<List<Change>> monitor, INotifier<Change> notifier,
            int period = 10000) : base(period)
        {
            _monitor = monitor;
            _notifier = notifier;
        }


        protected override async Task DoWork()
        {
            try
            {
                await _monitor.ResetStates(ResetTimeSpan);
                var changes = await _monitor.GetChanges(Percentage, MultipleChangesSpan, MultipleChanges);
                if(changes.Any())
                    await _notifier.SendNotificationMessage(changes);
            }
            catch (Exception e)
            {
                Console.WriteLine();
            }
        }
    }
}
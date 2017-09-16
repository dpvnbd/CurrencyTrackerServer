using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CurrencyTrackerServer.ChangeTrackerService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Entities.Changes;

namespace CurrencyTrackerServer.ChangeTrackerService.Concrete
{
    public class ChangeTimerWorker : AbstractTimerWorker
    {
        public IChangeMonitor<IEnumerable<Change>> Monitor { get; }
        private readonly INotifier<Change> _notifier;

        public int Percentage { get; set; }

        protected ChangeSource ChangeSource = ChangeSource.None;

        public TimeSpan ResetTimeSpan { get; set; }

        public bool MultipleChanges { get; set; }

        public TimeSpan MultipleChangesSpan { get; set; }

        public ChangeTimerWorker(IChangeMonitor<IEnumerable<Change>> monitor, INotifier<Change> notifier,
            int period = 10000) : base(period)
        {
            Monitor = monitor;
            _notifier = notifier;
        }


        protected override async Task DoWork()
        {
            try
            {
                await Monitor.ResetStates(ResetTimeSpan);
                var changes = await Monitor.GetChanges();
                if (changes.Any())
                    await _notifier.SendNotificationMessage(changes);
            }
            catch (Exception e)
            {
                var errorMessage = new Change
                {
                    Type = ChangeType.Error,
                    Message = e.Message,
                    Time = DateTime.Now,
                     ChangeSource = ChangeSource
                };

                await _notifier.SendNotificationMessage(errorMessage);
            }
        }
    }
}
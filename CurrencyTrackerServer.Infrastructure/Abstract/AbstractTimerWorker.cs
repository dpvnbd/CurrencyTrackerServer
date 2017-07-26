using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace CurrencyTrackerServer.Infrastructure.Abstract
{
    public abstract class AbstractTimerWorker<TChanges>
    {
        private readonly IChangeMonitor<TChanges> _monitor;
        private readonly INotifier<TChanges> _notifier;


        private readonly Timer _timer;
        private bool _enabled;

        protected AbstractTimerWorker(IChangeMonitor<TChanges> monitor, INotifier<TChanges> notifier, int period = 10000)
        {
            _monitor = monitor;
            _notifier = notifier;
            Period = period;
            _enabled = true;
            _timer = new Timer(TimerTick, null, period, Timeout.Infinite);
        }

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                if (!_enabled)
                {
                    _timer.Change(Timeout.Infinite, Timeout.Infinite);
                }
                else
                {
                    _timer.Change(Period, Timeout.Infinite);
                }
            }
        }

        public int Period { get; set; }

        public int Percentage { get; set; }

        public bool MultipleChanges { get; set; }

        public TimeSpan MultipleChangesSpan { get; set; }

        protected virtual async void TimerTick(object state)
        {
            var changes = await _monitor.GetChanges(Percentage, MultipleChangesSpan, MultipleChanges);
            await _notifier.SendNotificationMessage(changes);
            if (Enabled)
            {
                _timer.Change(Period, Timeout.Infinite);
            }
        }
    }
}
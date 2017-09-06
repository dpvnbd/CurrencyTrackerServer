using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CurrencyTrackerServer.Infrastructure.Abstract
{
    public abstract class AbstractTimerWorker
    {


        private readonly Timer _timer;
        private bool _enabled;

        protected AbstractTimerWorker(int period = 10000)
        {
            
            Period = period;
            _timer = new Timer(TimerTick, null, Timeout.Infinite, Timeout.Infinite);
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

        private async void TimerTick(object state)
        {
            await DoWork();
            if (Enabled)
            {
                _timer.Change(Period, Timeout.Infinite);
            }
        }

        protected abstract Task DoWork();
    }
}
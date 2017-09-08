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

        public bool Started { get; private set; }
        public int Period { get; set; }
        private object _lockObject = new object();


        protected AbstractTimerWorker(int period = 10000)
        {
            Period = period;
            _timer = new Timer(TimerTick, _timer, Timeout.Infinite, Timeout.Infinite);
        }

        public void Start()
        {
            if (!Started)
            {
                lock (_lockObject)
                {
                    if (!Started) //double check after lock to be thread safe
                    {
                        Started = true;
                        _timer.Change(Period, Timeout.Infinite);
                    }
                }
            }
        }

        public void Stop()
        {
            lock (_lockObject)
            {
                Started = false;
            }
        }

        private async void TimerTick(object state)
        {
            await DoWork();

            if (Started)
            {
                _timer.Change(Period, Timeout.Infinite);
            }
        }

        protected abstract Task DoWork();
    }
}
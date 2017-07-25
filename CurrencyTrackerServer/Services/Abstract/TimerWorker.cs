using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CurrencyTrackerServer.Services.Abstract
{
    public abstract class TimerWorker<TMessage>
    {
        private int _period;
        

        private Timer _timer;
        private bool _enabled;

        public TimerWorker(int period)
        {
            _period = period;
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
                    _timer.Change(_period, Timeout.Infinite);
                }
            }
        }

        public int Period
        {
            get { return _period; }
            set { _period = value; }
        }

        protected virtual void TimerTick(object state)
        {
            DoWork();
            if (Enabled)
            {
                _timer.Change(_period, Timeout.Infinite);
            }
        }

        protected abstract void DoWork();
    }
}
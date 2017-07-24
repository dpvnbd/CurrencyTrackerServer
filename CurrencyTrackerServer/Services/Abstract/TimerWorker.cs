using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CurrencyTrackerServer.Services.Abstract
{
    public abstract class TimerWorker<TMessage>
    {
        private int _period;

        public event EventHandler<TMessage> Message;
        public event EventHandler<string> InfoMessage;

        private Timer _timer;
        private bool _enabled;

        public TimerWorker(int period)
        {
            _period = period;
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
                    _timer.Change(-1, -1);
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

        protected virtual void SendMessage(TMessage model)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            var handler = Message;
            handler?.Invoke(this, model);
        }

        protected virtual void SendMessage(string message)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            var handler = InfoMessage;
            handler?.Invoke(this, message);
        }
    }
}
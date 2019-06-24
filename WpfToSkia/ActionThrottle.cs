using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Threading;

namespace WpfToSkia
{
    /// <summary>
    /// Represents an action execution throttling mechanism.
    /// </summary>
    public class ActionThrottle
    {
        private Timer _timer;
        private Action _action;
        private Dispatcher _dispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionThrottle"/> class.
        /// </summary>
        /// <param name="interval">The interval.</param>
        /// <param name="dispatcher">The dispatcher.</param>
        public ActionThrottle(TimeSpan interval, Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;

            _timer = new Timer(interval.TotalMilliseconds);
            _timer.Enabled = true;
            _timer.Stop();
            _timer.Elapsed += _timer_Elapsed;
        }

        /// <summary>
        /// Resets the current timer and replaces the action to be invoked.
        /// </summary>
        /// <param name="action">The action.</param>
        public void ResetReplace(Action action)
        {
            if (_timer != null)
            {
                _action = action;

                if (!_timer.Enabled)
                {
                    _timer.Start();
                }
            }
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_action != null)
            {
                _dispatcher.Invoke(() =>
                {
                    _action?.Invoke();
                });
                _action = null;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer = null;
            }
        }
    }
}

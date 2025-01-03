using System.Diagnostics;

namespace LiveSearchEngine.Common
{
    /// <summary>
    /// Loop time-interval class.
    /// </summary>
    public class Interval
    {
        public Interval(double delay)
        {
            Delay = delay;
            _sw = Stopwatch.StartNew();
        }

        /// <summary>
        /// Your delay.
        /// </summary>
        public double Delay { get; }

        /// <summary>
        /// Checks if the delay has expired and restarts the timer (so you can reuse this property).
        /// </summary>
        public bool Elapsed
        {
            get
            {
                lock (_lock)
                {
                    if (_sw.ElapsedMilliseconds < Delay)
                        return false;

                    _sw.Restart();
                    return true;
                }
            }
        }

        /// <summary>
        /// Returns the time remaining before elapsed.
        /// </summary>
        public double TimeLeft
        {
            get
            {
                lock (_lock)
                {
                    double timeLeft = Delay - _sw.ElapsedMilliseconds;
                    if (timeLeft < 0)
                        return 0;

                    return timeLeft;
                }
            }
        }

        private readonly object _lock = new object();
        private readonly Stopwatch _sw;
    }
}
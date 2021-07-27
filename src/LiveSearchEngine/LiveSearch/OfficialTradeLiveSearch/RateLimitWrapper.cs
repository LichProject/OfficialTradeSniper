using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LiveSearchEngine.Common;
using LiveSearchEngine.Models;

namespace LiveSearchEngine.LiveSearch.OfficialTradeLiveSearch
{
    /// <summary>
    /// Per requests delay wrapper.
    /// </summary>
    public class RateLimitWrapper
    {
        const int DefaultSleepStep = 20;

        /// <summary>
        /// Set the interval to 1000 ms.
        /// </summary>
        public RateLimitWrapper()
        {
            _interval = new Interval(1000);
        }

        /// <summary>
        /// Change the delay interval.
        /// </summary>
        /// <param name="xRateLimitIpHeaderValue">X-Rate-Limit-Ip header value (format is XX:XX:XX).</param>
        /// <param name="factor">Delay factor.</param>
        public void ChangeInterval(string xRateLimitIpHeaderValue, double factor = 1.0)
        {
            var rates = ParseHeader(xRateLimitIpHeaderValue);
            var delay = rates.Max(x => x.BestDelayMs);
            _interval = new Interval(delay * factor);
        }

        /// <summary>
        /// Wait until the delay between requests expires (Task.Delay).
        /// </summary>
        public void Wait()
        {
#if DEBUG
            Console.WriteLine(_interval.TimeLeft);
#endif

            while (!_interval.Elapsed)
                Thread.Sleep(DefaultSleepStep);
        }

        IEnumerable<RateLimit> ParseHeader(string header)
        {
            if (header == null)
                yield break;

            var rates = header.Split(',');
            foreach (var rate in rates)
            {
                var split = rate.Split(':');

                int.TryParse(split[0], out int requests);
                int.TryParse(split[1], out int seconds);
                int.TryParse(split[2], out int timeout);

                yield return new RateLimit(requests, seconds, timeout);
            }
        }

        Interval _interval;
    }
}
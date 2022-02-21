using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LiveSearchEngine.Common;
using LiveSearchEngine.Interfaces;
using LiveSearchEngine.Models;

namespace LiveSearchEngine.LiveSearch.OfficialTradeLiveSearch
{
    /// <summary>
    /// Per requests delay wrapper.
    /// </summary>
    public class RateLimitWrapper : IRateLimit
    {
        const int DefaultSleepStep = 10;

        /// <summary>
        /// Set the interval to 1000 ms.
        /// </summary>
        public RateLimitWrapper()
        {
            _interval = new Interval(1000);
        }

        public IReadOnlyList<RateLimit> AccountRate { get; private set; }
        public IReadOnlyList<RateLimit> IpRate { get; private set; }

        public void ChangeInterval(params object[] args)
        {
            AccountRate = ParseHeader(args.ElementAtOrDefault(0) as string).ToList();
            IpRate = ParseHeader(args.ElementAtOrDefault(1) as string).ToList();
            
            var delay = AccountRate.Union(IpRate).Max(x => x.TotalDelayMs) + 100;
            if (_interval != null && Math.Abs(_interval.Delay - delay) > 0)
            {
                _interval = new Interval(delay);
            }
        }

        /// <summary>
        /// Wait delay between requests.
        /// </summary>
        public void Wait(int overridenDelay = -1)
        {
#if DEBUG
            Console.WriteLine(_interval.TimeLeft);
#endif

            if (overridenDelay > -1)
            {
                Thread.Sleep(overridenDelay);
            }
            else
            {
                while (!_interval.Elapsed)
                    Thread.Sleep(DefaultSleepStep);
            }
        }

        /// <inheritdoc cref="Wait"/>
        public async Task WaitAsync()
        {
#if DEBUG
            Console.WriteLine(_interval.TimeLeft);
#endif

            while (!_interval.Elapsed)
                await Task.Delay(DefaultSleepStep);
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
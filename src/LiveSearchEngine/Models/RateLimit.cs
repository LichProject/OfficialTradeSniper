namespace LiveSearchEngine.Models
{
    public class RateLimit
    {
        public RateLimit(int totalRequests, int totalSeconds, int timeout)
        {
            TotalRequests = totalRequests;
            TotalSeconds = totalSeconds;
            Timeout = timeout;
        }

        public int TotalRequests { get; }
        public int TotalSeconds { get; }
        public int Timeout { get; }

        public double TotalDelayMs => TotalSeconds / (double) TotalRequests * 1000;
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using LiveSearchEngine.Models;

namespace LiveSearchEngine.Interfaces
{
    public interface IRateLimit
    {
        IReadOnlyList<RateLimit> AccountRate { get; }
        IReadOnlyList<RateLimit> IpRate { get; }

        void ChangeInterval(params object[] args);
        void Wait(int overridenDelay = -1);
        Task WaitAsync();
    }
}
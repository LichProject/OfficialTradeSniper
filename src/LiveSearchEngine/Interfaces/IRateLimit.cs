using System.Threading.Tasks;

namespace LiveSearchEngine.Interfaces
{
    public interface IRateLimit
    {
        void ChangeInterval(params object[] args);
        void Wait();
        Task WaitAsync();
    }
}
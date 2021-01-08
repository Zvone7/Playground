using System.Threading.Tasks;

namespace WorkProvider.Infrastructrue
{
    public interface IWorkDequeuer
    {
        Task<IWork> DequeueAsync();
    }
}

using System.Threading;
using System.Threading.Tasks;

namespace WorkProvider.Infrastructrue
{
    public interface IWorkProvider
    {
        Task RunAsync(CancellationToken cancellationToken);
    }
}

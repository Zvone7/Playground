using System.Threading.Tasks;

namespace WorkProvider.Infrastructrue
{
    public interface IFileWriter
    {
        Task WriteAsync();
    }
}

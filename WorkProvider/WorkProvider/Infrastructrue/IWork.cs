using System;
using System.Threading.Tasks;

namespace WorkProvider.Infrastructrue
{
    public interface IWork
    {
        Task Execute();
    }
}

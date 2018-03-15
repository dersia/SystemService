using System.Threading;
using System.Threading.Tasks;

namespace SystemService
{
    public interface ISystemService
    {
        Task Start(CancellationToken cancellationToken);
        Task Stop();
    }
}

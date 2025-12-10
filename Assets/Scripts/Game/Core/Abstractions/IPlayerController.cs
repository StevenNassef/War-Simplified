using System.Threading;
using System.Threading.Tasks;

namespace Game.Core.Abstractions
{
    public interface IPlayerController
    {
        Task RequestDrawAsync(CancellationToken cancellationToken = default);
    }
}
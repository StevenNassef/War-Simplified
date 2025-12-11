using System.Threading;
using System.Threading.Tasks;

namespace Game.Core.Abstractions
{
    public interface IGameController
    {
        
        Task StartGameAsync(CancellationToken cancellationToken = default);
        Task PlayRoundAsync(CancellationToken cancellationToken = default);
        
    }
}
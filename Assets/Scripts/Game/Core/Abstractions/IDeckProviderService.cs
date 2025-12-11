using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Game.Core.Model;

namespace Game.Core.Abstractions
{
    public interface IDeckProviderService
    {
        Task InitializeAsync(CancellationToken cancellationToken = default);
        
        Task<IReadOnlyList<Card>> DrawCardsAsync(int count = 1, CancellationToken cancellationToken = default);
        
        Task ReshuffleAsync(CancellationToken cancellationToken = default);
    }
}
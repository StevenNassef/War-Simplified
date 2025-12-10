using System.Collections.Generic;
using Game.Core.Model;

namespace Game.Core.Abstractions
{
    public interface IGameMode
    {
        int PointsPerRound { get; }
        
        void ConfigureNewGame(GameState gameState, IReadOnlyList<IPlayer> players);
        int ResolveRoundWinner(GameState gameState, IReadOnlyList<IPlayer> players, IReadOnlyList<Card> roundCards);
        bool TryResolveGameWinner(GameState gameState, IReadOnlyList<IPlayer> players, out int winnerIndex);
    }
}
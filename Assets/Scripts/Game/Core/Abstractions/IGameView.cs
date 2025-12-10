using System.Collections.Generic;
using Game.Core.Model;

namespace Game.Core.Abstractions
{
    public interface IGameView
    {
        void StartGame();
        void UpdateScores(IReadOnlyList<int> scores);
        void ShowRoundResult(int roundIndex, IReadOnlyList<Card> cards, IReadOnlyList<int> scores, int roundWinnerId);
        void ShowGameOver(int winnerId);
        void EndGame();
    }
    
}
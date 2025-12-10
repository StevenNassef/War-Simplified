using System.Collections.Generic;
using System.Linq;
using Game.Core.Abstractions;
using Game.Core.Model;

namespace Game.Core.Application.GameModes
{
    public class SimpleWasGameMode : IGameMode
    {
        public int PointsPerRound { get; }

        private readonly int _initialMaxRounds;

        public SimpleWasGameMode(int initialMaxRounds = 8, int pointsPerRound = 1)
        {
            _initialMaxRounds = initialMaxRounds;
            PointsPerRound = pointsPerRound;
        }
        public void ConfigureNewGame(GameState gameState, IReadOnlyList<IPlayer> players)
        {
            gameState.MaxRounds = _initialMaxRounds;
            gameState.CurrentRound = 0;
            gameState.Phase = GamePhase.NotStarted;

            // Initialize player states
            gameState.Players = new PlayerState[players.Count];

            for (var i = 0; i < gameState.Players.Length; i++)
            {
                gameState.Players[i] = new PlayerState();
            }
        }

        public int EvaluateRoundOutcome(GameState gameState, IReadOnlyList<IPlayer> players, IReadOnlyList<Card> roundCards)
        {
            // if the count of cards in the round is not equal to the number of players, the game is over
            if (roundCards.Count != players.Count)
            {
                gameState.Phase = GamePhase.GameOver;
                return -1;
            }
            
            var winnerIndex = roundCards.GetUniqueHighestItemByIndex(Card.ValueOnlyComparerInstance, out _);

            if (winnerIndex != -1)
            {
                gameState.Players[winnerIndex].Score += PointsPerRound;
            }
            
            gameState.CurrentRound++;
            return winnerIndex;
        }

        public bool TryResolveGameWinner(GameState gameState, IReadOnlyList<IPlayer> players, out int winnerIndex)
        {
            if (gameState.CurrentRound >= gameState.MaxRounds)
            {
                gameState.Phase = GamePhase.GameOver;
                winnerIndex = gameState.PlayerScores.ToList().GetUniqueHighestItemByIndex(out _);
                return true;
            }
            
            winnerIndex = -1;
            return false;
        }
    }
}

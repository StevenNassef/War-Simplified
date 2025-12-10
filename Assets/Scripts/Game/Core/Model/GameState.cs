using System.Collections.Generic;
using System.Linq;

namespace Game.Core.Model
{
    public class GameState
    {
        public int CurrentRound { get; set; }
        public int MaxRounds { get; set; }
        public GamePhase Phase { get; set; } = GamePhase.NotStarted;
        public PlayerState[] Players { get; set; }
        
        public IEnumerable<int> PlayerScores => Players.Select(p => p.Score);
    }

    public class PlayerState
    {
        public int Score { get; set; } = 0;
    }


    public enum GamePhase
    {
        NotStarted,
        Playing,
        GameOver,
        Finished
    }
    
    
}
namespace Game.Core.Model
{
    public class GameState
    {
        public int CurrentRound { get; set; }
        public int MaxRounds { get; set; }
        
        public PlayerState[] Players { get; set; }
    }

    public class PlayerState
    {
        public int Score { get; set; } = 0;
    }
    
}
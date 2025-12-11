using Game.Core.Abstractions;
using TMPro;
using UnityEngine;

namespace Game.Client.Runtime
{
    /// <summary>
    /// Component that displays a player's name and score.
    /// </summary>
    public class PlayerScoreDisplay : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI playerNameText;
        
        [SerializeField]
        private TextMeshProUGUI scoreText;
        
        [SerializeField, Tooltip("Format string for score display. {0} will be replaced with the score value.")]
        private string scoreFormat = "Score: {0}";

        /// <summary>
        /// Initializes the display with player information.
        /// </summary>
        /// <param name="player">The player to display information for</param>
        public void Initialize(IPlayer player)
        {
            if (player == null)
            {
                Debug.LogWarning("[PlayerScoreDisplay] Player is null");
                return;
            }
            
            SetPlayerName(player.DisplayName);
            SetScore(0);
        }

        /// <summary>
        /// Sets the player name displayed.
        /// </summary>
        /// <param name="playerName">The name to display</param>
        public void SetPlayerName(string playerName)
        {
            if (playerNameText)
            {
                playerNameText.text = playerName;
            }
        }

        /// <summary>
        /// Sets the score displayed.
        /// </summary>
        /// <param name="score">The score value to display</param>
        public void SetScore(int score)
        {
            if (scoreText)
            {
                scoreText.text = string.Format(scoreFormat, score);
            }
        }

        /// <summary>
        /// Updates both name and score at once.
        /// </summary>
        /// <param name="playerName">The name to display</param>
        /// <param name="score">The score value to display</param>
        public void UpdateDisplay(string playerName, int score)
        {
            SetPlayerName(playerName);
            SetScore(score);
        }
    }
}


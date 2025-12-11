using System.Collections.Generic;
using System.Threading.Tasks;
using Game.Core.Abstractions;
using Game.Core.Model;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.Client.Runtime
{
    /// <summary>
    /// Unity implementation of IGameView for displaying game state and results.
    /// </summary>
    public class GameView : GameViewBase
    {
        [Header("Player Score Displays")] [SerializeField] [Tooltip("PlayerScoreDisplay components (one per player)")]
        private PlayerScoreDisplay[] playerScoreDisplays;

        [Header("Round Result Display")] [SerializeField] [Tooltip("Parent GameObject for round result display")]
        private GameObject roundResultPanel;

        [SerializeField] [Tooltip("TextMeshPro component for displaying round number")]
        private TextMeshProUGUI roundNumberText;

        [SerializeField] [Tooltip("for displaying cards (one per player)")]
        private TextMeshProUGUI[] cardTexts;

        [SerializeField] [Tooltip("for displaying round winner")]
        private TextMeshProUGUI roundWinnerText;
        
        [Header("Main Menu Panel")] [SerializeField]
        private GameObject mainMenuPanel;

        [Header("Game Panel")] [SerializeField] [Tooltip("Parent GameObject for main panel")] 
        private GameObject gamePanel;
        
        [Header("Game Over Display")] [SerializeField] [Tooltip("Parent GameObject for game over display")]
        private GameObject gameOverPanel;

        [SerializeField] [Tooltip("for displaying game over message")]
        private TextMeshProUGUI gameOverText;

        [SerializeField] private GameObject playerControls;
        
        [SerializeField] private Button restartButton;

        private void Awake()
        {
            restartButton.onClick.AddListener(RestartGameScene);
        }

        private void OnDisable()
        {
            if (!restartButton) return;
            restartButton.onClick.RemoveAllListeners();
        }

        public override void Initialize(IPlayer[] players)
        {
            Players = players;

            // Validate that we have enough UI elements for the number of players
            if (playerScoreDisplays != null && playerScoreDisplays.Length < players.Length)
                Debug.LogWarning(
                    $"[GameView] Not enough player score displays ({playerScoreDisplays.Length}) for players ({players.Length})");

            if (cardTexts != null && cardTexts.Length < players.Length)
                Debug.LogWarning(
                    $"[GameView] Not enough card texts ({cardTexts.Length}) for players ({players.Length})");

            // Initialize player score displays
            if (playerScoreDisplays == null || Players == null)
            {
                Debug.LogWarning("[GameView] Player score displays not initialized");
                return;
            }

            for (var i = 0; i < playerScoreDisplays.Length && i < Players.Length; i++)
                playerScoreDisplays[i].Initialize(Players[i]);
            
            // Initialize round text
            roundNumberText.text = "Round 1";
            roundWinnerText.text = string.Empty;
        }

        protected override async Task StartGameInternal()
        {
            await Task.Yield();
            gamePanel.SetActive(true);
            mainMenuPanel.SetActive(false);
            roundResultPanel.SetActive(true);
            gameOverPanel.SetActive(false);
            playerControls.gameObject.SetActive(true);
        }

        protected override async Task UpdateScoresInternal(IReadOnlyList<int> scores)
        {
            await Task.Yield();

            for (var i = 0; i < playerScoreDisplays.Length && i < scores.Count; i++)
                playerScoreDisplays[i].SetScore(scores[i]);
        }

        protected override async Task ShowRoundResultInternal(int roundIndex, IReadOnlyList<Card> cards,
            IReadOnlyList<int> scores,
            int roundWinnerId)
        {
            playerControls.gameObject.SetActive(false);
            await Task.Yield();
            
            // Display round number
            roundNumberText.text = $"Round {roundIndex}";

            // Display cards
            for (var i = 0; i < cardTexts.Length && i < cards.Count; i++)
            {
                var card = cards[i];
                cardTexts[i].text = FormatCard(card);
            }

            // Display round winner
            if (roundWinnerId >= 0 && roundWinnerId < cards.Count)
            {
                var winnerCard = cards[roundWinnerId];
                var winnerName = Players != null && roundWinnerId < Players.Length
                    ? Players[roundWinnerId].DisplayName
                    : $"Player {roundWinnerId + 1}";
                roundWinnerText.text = $"{winnerName} wins with {FormatCard(winnerCard)}!";
            }
            else
            {
                roundWinnerText.text = "Tie!";
            }
            
            await Task.Delay(2000);
            
            roundWinnerText.text = string.Empty;
            
            // Clear cards
            for (var i = 0; i < cardTexts.Length && i < cards.Count; i++)
            {
                cardTexts[i].text = string.Empty;
            }
            
            playerControls.gameObject.SetActive(true);
            // Display round number
            roundNumberText.text = $"Round {roundIndex + 1}";
        }

        protected override async Task ShowGameOverInternal(int winnerId)
        {
            roundResultPanel.SetActive(false);
            
            await Task.Yield();

            gameOverPanel.SetActive(true);

            switch (winnerId)
            {
                case >= 0 when Players != null && winnerId < Players.Length:
                {
                    var winnerName = Players[winnerId].DisplayName;
                    gameOverText.text = $"Game Over!\n{winnerName} Wins!";
                    break;
                }
                case >= 0:
                    gameOverText.text = $"Game Over!\nPlayer {winnerId + 1} Wins!";
                    break;
                default:
                    gameOverText.text = "Game Over!\nIt's a Tie!";
                    break;
            }
        }

        protected override async Task EndGameInternal()
        {
            await Task.Yield();

            gameOverPanel.SetActive(true);

            roundResultPanel.SetActive(false);

            if (gameOverText && string.IsNullOrEmpty(gameOverText.text)) gameOverText.text = "Game Over!";
        }

        private static void RestartGameScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        #region Helper methods

        protected static string FormatCard(Card card)
        {
            var valueString = card.CardValue switch
            {
                CardValue.Two => "2",
                CardValue.Three => "3",
                CardValue.Four => "4",
                CardValue.Five => "5",
                CardValue.Six => "6",
                CardValue.Seven => "7",
                CardValue.Eight => "8",
                CardValue.Nine => "9",
                CardValue.Ten => "10",
                CardValue.Jack => "J",
                CardValue.Queen => "Q",
                CardValue.King => "K",
                CardValue.Ace => "A",
                _ => card.CardValue.ToString()
            };

            var suitSymbol = card.Suit switch
            {
                CardSuit.Spades => "♠",
                CardSuit.Hearts => "♥",
                CardSuit.Diamonds => "♦",
                CardSuit.Clubs => "♣",
                _ => card.Suit.ToString()
            };

            return $"{valueString}{suitSymbol}";
        }

        #endregion
    }
}
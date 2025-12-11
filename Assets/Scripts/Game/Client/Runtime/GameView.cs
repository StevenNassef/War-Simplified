using System.Collections.Generic;
using System.Threading.Tasks;
using Game.Core.Abstractions;
using Game.Core.Model;
using UnityEngine;
using TMPro;

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

        [Header("Game Over Display")] [SerializeField] [Tooltip("Parent GameObject for game over display")]
        private GameObject gameOverPanel;

        [SerializeField] [Tooltip("for displaying game over message")]
        private TextMeshProUGUI gameOverText;
        

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
        }

        protected override async Task StartGameInternal()
        {
            await Task.Yield();

            roundResultPanel.SetActive(false);

            gameOverPanel.SetActive(false);
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
            await Task.Yield();

            // Show round result panel
            roundResultPanel.SetActive(true);

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

            // Update scores
            await UpdateScoresInternal(scores);
        }

        protected override async Task ShowGameOverInternal(int winnerId)
        {
            await Task.Yield();

            gameOverPanel.SetActive(true);

            roundResultPanel.SetActive(false);

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
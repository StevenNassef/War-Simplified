using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using Game.Core.Abstractions;
using Game.Core.Application.GameModes;
using Game.Core.Model;

namespace Game.Core.Tests.Application.GameModes
{
    [TestFixture]
    public class SimpleWasGameModeTests
    {
        private SimpleWasGameMode _gameMode;
        private GameState _gameState;
        private List<IPlayer> _players;

        [SetUp]
        public void SetUp()
        {
            _gameMode = new SimpleWasGameMode(initialMaxRounds: 8, pointsPerRound: 1);
            _gameState = new GameState();
            
            var player1 = Substitute.For<IPlayer>();
            player1.Id.Returns("player1");
            player1.DisplayName.Returns("Player 1");
            
            var player2 = Substitute.For<IPlayer>();
            player2.Id.Returns("player2");
            player2.DisplayName.Returns("Player 2");
            
            _players = new List<IPlayer> { player1, player2 };
        }

        [Test]
        public void ConfigureNewGame_InitializesGameStateCorrectly()
        {
            // Act
            _gameMode.ConfigureNewGame(_gameState, _players);

            // Assert
            Assert.AreEqual(8, _gameState.MaxRounds);
            Assert.AreEqual(0, _gameState.CurrentRound);
            Assert.AreEqual(GamePhase.Playing, _gameState.Phase);
            Assert.IsNotNull(_gameState.Players);
            Assert.AreEqual(2, _gameState.Players.Length);
            Assert.AreEqual(0, _gameState.Players[0].Score);
            Assert.AreEqual(0, _gameState.Players[1].Score);
        }

        [Test]
        public void ConfigureNewGame_WithThreePlayers_CreatesThreePlayerStates()
        {
            // Arrange
            var player3 = Substitute.For<IPlayer>();
            player3.Id.Returns("player3");
            player3.DisplayName.Returns("Player 3");
            _players.Add(player3);

            // Act
            _gameMode.ConfigureNewGame(_gameState, _players);

            // Assert
            Assert.AreEqual(3, _gameState.Players.Length);
        }

        [Test]
        public void EvaluateRoundOutcome_WithValidCards_ReturnsWinnerIndex()
        {
            // Arrange
            _gameMode.ConfigureNewGame(_gameState, _players);
            _gameState.Phase = GamePhase.Playing;
            var roundCards = new List<Card>
            {
                new(CardSuit.Hearts, CardValue.Two),
                new(CardSuit.Spades, CardValue.Ace)
            };

            // Act
            var winnerIndex = _gameMode.EvaluateRoundOutcome(_gameState, _players, roundCards);

            // Assert
            Assert.AreEqual(1, winnerIndex); // Ace beats Two
            Assert.AreEqual(1, _gameState.Players[1].Score);
            Assert.AreEqual(0, _gameState.Players[0].Score);
        }

        [Test]
        public void EvaluateRoundOutcome_WithCardCountMismatch_SetsGameOver()
        {
            // Arrange
            _gameMode.ConfigureNewGame(_gameState, _players);
            _gameState.Phase = GamePhase.Playing;
            var roundCards = new List<Card>
            {
                new(CardSuit.Hearts, CardValue.Two)
                // Only one card for two players
            };

            // Act
            var winnerIndex = _gameMode.EvaluateRoundOutcome(_gameState, _players, roundCards);

            // Assert
            Assert.AreEqual(-1, winnerIndex);
            Assert.AreEqual(GamePhase.GameOver, _gameState.Phase);
        }

        [Test]
        public void EvaluateRoundOutcome_WithTies_ReturnsNegativeOne()
        {
            // Arrange
            _gameMode.ConfigureNewGame(_gameState, _players);
            _gameState.Phase = GamePhase.Playing;
            var roundCards = new List<Card>
            {
                new(CardSuit.Hearts, CardValue.Ace),
                new(CardSuit.Spades, CardValue.Ace)
            };

            // Act
            var winnerIndex = _gameMode.EvaluateRoundOutcome(_gameState, _players, roundCards);

            // Assert
            Assert.AreEqual(-1, winnerIndex);
            Assert.AreEqual(0, _gameState.Players[0].Score);
            Assert.AreEqual(0, _gameState.Players[1].Score);
        }

        [Test]
        public void EvaluateRoundOutcome_UpdatesCurrentRound()
        {
            // Arrange
            _gameMode.ConfigureNewGame(_gameState, _players);
            _gameState.Phase = GamePhase.Playing;
            _gameState.CurrentRound = 3;
            var roundCards = new List<Card>
            {
                new(CardSuit.Hearts, CardValue.Two),
                new(CardSuit.Spades, CardValue.Ace)
            };

            // Act
            _gameMode.EvaluateRoundOutcome(_gameState, _players, roundCards);

            // Assert
            Assert.AreEqual(4, _gameState.CurrentRound);
        }

        [Test]
        public void EvaluateRoundOutcome_WithCustomPointsPerRound_AwardsCorrectPoints()
        {
            // Arrange
            var customGameMode = new SimpleWasGameMode(initialMaxRounds: 8, pointsPerRound: 5);
            customGameMode.ConfigureNewGame(_gameState, _players);
            _gameState.Phase = GamePhase.Playing;
            var roundCards = new List<Card>
            {
                new(CardSuit.Hearts, CardValue.Two),
                new(CardSuit.Spades, CardValue.Ace)
            };

            // Act
            customGameMode.EvaluateRoundOutcome(_gameState, _players, roundCards);

            // Assert
            Assert.AreEqual(5, _gameState.Players[1].Score);
            Assert.AreEqual(0, _gameState.Players[0].Score);
        }

        [Test]
        public void TryResolveGameWinner_WhenMaxRoundsReached_ReturnsTrue()
        {
            // Arrange
            _gameMode.ConfigureNewGame(_gameState, _players);
            _gameState.CurrentRound = 8;
            _gameState.Phase = GamePhase.Playing;

            // Act
            var result = _gameMode.TryResolveGameWinner(_gameState, _players, out var winnerIndex);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(GamePhase.GameOver, _gameState.Phase);
        }

        [Test]
        public void TryResolveGameWinner_WhenMaxRoundsNotReached_ReturnsFalse()
        {
            // Arrange
            _gameMode.ConfigureNewGame(_gameState, _players);
            _gameState.CurrentRound = 5;
            _gameState.Phase = GamePhase.Playing;

            // Act
            var result = _gameMode.TryResolveGameWinner(_gameState, _players, out var winnerIndex);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(-1, winnerIndex);
        }

        [Test]
        public void TryResolveGameWinner_WithTiedScores_ReturnsNegativeOne()
        {
            // Arrange
            _gameMode.ConfigureNewGame(_gameState, _players);
            _gameState.CurrentRound = 8;
            _gameState.Phase = GamePhase.Playing;
            _gameState.Players[0].Score = 5;
            _gameState.Players[1].Score = 5;

            // Act
            var result = _gameMode.TryResolveGameWinner(_gameState, _players, out var winnerIndex);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(-1, winnerIndex); // Tie
        }

        [Test]
        public void TryResolveGameWinner_WithUniqueHighestScore_ReturnsWinnerIndex()
        {
            // Arrange
            _gameMode.ConfigureNewGame(_gameState, _players);
            _gameState.CurrentRound = 8;
            _gameState.Phase = GamePhase.Playing;
            _gameState.Players[0].Score = 3;
            _gameState.Players[1].Score = 5;

            // Act
            var result = _gameMode.TryResolveGameWinner(_gameState, _players, out var winnerIndex);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, winnerIndex);
        }

        [Test]
        public void PointsPerRound_ReturnsConfiguredValue()
        {
            // Arrange & Act
            var gameMode1 = new SimpleWasGameMode(initialMaxRounds: 8, pointsPerRound: 1);
            var gameMode2 = new SimpleWasGameMode(initialMaxRounds: 8, pointsPerRound: 5);

            // Assert
            Assert.AreEqual(1, gameMode1.PointsPerRound);
            Assert.AreEqual(5, gameMode2.PointsPerRound);
        }
    }
}


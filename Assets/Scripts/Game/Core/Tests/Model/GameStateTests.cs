using NUnit.Framework;
using Game.Core.Model;
using System.Linq;

namespace Game.Core.Tests.Model
{
    [TestFixture]
    public class GameStateTests
    {
        [Test]
        public void GameState_Initialization_SetsDefaultValues()
        {
            // Arrange & Act
            var gameState = new GameState();

            // Assert
            Assert.AreEqual(0, gameState.CurrentRound);
            Assert.AreEqual(0, gameState.MaxRounds);
            Assert.AreEqual(GamePhase.NotStarted, gameState.Phase);
            Assert.IsNull(gameState.Players);
        }

        [Test]
        public void GameState_Properties_CanBeSet()
        {
            // Arrange
            var gameState = new GameState
            {
                // Act
                CurrentRound = 5,
                MaxRounds = 10,
                Phase = GamePhase.Playing,
                Players = new PlayerState[2]
            };

            // Assert
            Assert.AreEqual(5, gameState.CurrentRound);
            Assert.AreEqual(10, gameState.MaxRounds);
            Assert.AreEqual(GamePhase.Playing, gameState.Phase);
            Assert.IsNotNull(gameState.Players);
            Assert.AreEqual(2, gameState.Players.Length);
        }

        [Test]
        public void PlayerState_Initialization_SetsDefaultScore()
        {
            // Arrange & Act
            var playerState = new PlayerState();

            // Assert
            Assert.AreEqual(0, playerState.Score);
        }

        [Test]
        public void PlayerState_Score_CanBeUpdated()
        {
            // Arrange
            var playerState = new PlayerState
            {
                // Act
                Score = 10
            };

            // Assert
            Assert.AreEqual(10, playerState.Score);
        }

        [Test]
        public void GameState_PlayerScores_ReturnsCorrectScores()
        {
            // Arrange
            var gameState = new GameState
            {
                Players = new[]
                {
                    new PlayerState { Score = 5 },
                    new PlayerState { Score = 10 },
                    new PlayerState { Score = 3 }
                }
            };

            // Act
            var scores = gameState.PlayerScores.ToList();

            // Assert
            Assert.AreEqual(3, scores.Count);
            Assert.AreEqual(5, scores[0]);
            Assert.AreEqual(10, scores[1]);
            Assert.AreEqual(3, scores[2]);
        }

        [Test]
        public void GamePhase_EnumValues_AreDefined()
        {
            // Assert
            Assert.IsTrue(System.Enum.IsDefined(typeof(GamePhase), GamePhase.NotStarted));
            Assert.IsTrue(System.Enum.IsDefined(typeof(GamePhase), GamePhase.Playing));
            Assert.IsTrue(System.Enum.IsDefined(typeof(GamePhase), GamePhase.GameOver));
            Assert.IsTrue(System.Enum.IsDefined(typeof(GamePhase), GamePhase.Finished));
        }
    }
}


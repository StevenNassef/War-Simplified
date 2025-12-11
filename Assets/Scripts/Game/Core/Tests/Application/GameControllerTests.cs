using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using NSubstitute;
using Game.Core.Abstractions;
using Game.Core.Application;
using Game.Core.Application.GameModes;
using Game.Core.Model;

namespace Game.Core.Tests.Application
{
    [TestFixture]
    public class GameControllerTests
    {
        private IDeckProviderService _deckProvider;
        private IGameView _gameView;
        private GameState _gameState;
        private IGameMode _gameMode;
        private IPlayer[] _players;
        private IPlayerController[] _playerControllers;
        private ILogger _logger;

        [SetUp]
        public void SetUp()
        {
            _deckProvider = Substitute.For<IDeckProviderService>();
            _gameView = Substitute.For<IGameView>();
            _logger = Substitute.For<ILogger>();
            _gameState = new GameState();
            _gameMode = new SimpleWasGameMode(initialMaxRounds: 8, pointsPerRound: 1);
            
            var player1 = Substitute.For<IPlayer>();
            player1.Id.Returns("player1");
            player1.DisplayName.Returns("Player 1");
            
            var player2 = Substitute.For<IPlayer>();
            player2.Id.Returns("player2");
            player2.DisplayName.Returns("Player 2");
            
            _players = new[] { player1, player2 };
            
            _playerControllers = new[]
            {
                Substitute.For<IPlayerController>(),
                Substitute.For<IPlayerController>()
            };
        }

        [Test]
        public void Constructor_WithMismatchedPlayerCounts_ThrowsArgumentException()
        {
            // Arrange
            var mismatchedControllers = new[] { Substitute.For<IPlayerController>() };

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
            {
                _ = new GameController(_deckProvider, _players, mismatchedControllers, _gameView, _gameMode, _gameState, _logger);
            });
        }

        [Test]
        public void Constructor_WithMatchingCounts_CreatesInstance()
        {
            // Act
            var controller = new GameController(_deckProvider, _players, _playerControllers, _gameView, _gameMode, _gameState, _logger);

            // Assert
            Assert.IsNotNull(controller);
        }

        [Test]
        public async Task StartGameAsync_InitializesGameCorrectly()
        {
            // Arrange
            _deckProvider.InitializeAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
            var controller = new GameController(_deckProvider, _players, _playerControllers, _gameView, _gameMode, _gameState, _logger);

            // Act
            await controller.StartGameAsync();

            // Assert
            await _deckProvider.Received(1).InitializeAsync(Arg.Any<CancellationToken>());
            _gameView.Received(1).StartGame();
            _gameView.Received().UpdateScores(Arg.Any<List<int>>());
            Assert.AreEqual(GamePhase.Playing, _gameState.Phase);
        }

        [Test]
        public async Task StartGameAsync_WhenAlreadyFinished_DoesNotRestart()
        {
            // Arrange
            _deckProvider.InitializeAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
            var controller = new GameController(_deckProvider, _players, _playerControllers, _gameView, _gameMode, _gameState, _logger);
            _gameState.Phase = GamePhase.Finished;
            _gameView.ClearReceivedCalls();

            // Act
            await controller.StartGameAsync();

            // Assert
            _gameView.DidNotReceive().StartGame();
        }

        [Test]
        public async Task PlayRoundAsync_WhenGameNotStarted_DoesNotPlay()
        {
            // Arrange
            var controller = new GameController(_deckProvider, _players, _playerControllers, _gameView, _gameMode, _gameState, _logger);
            _gameState.Phase = GamePhase.NotStarted;

            // Act
            await controller.PlayRoundAsync();

            // Assert
            await _deckProvider.DidNotReceive().DrawCardsAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task PlayRoundAsync_WhenGameFinished_DoesNotPlay()
        {
            // Arrange
            var controller = new GameController(_deckProvider, _players, _playerControllers, _gameView, _gameMode, _gameState, _logger);
            _gameState.Phase = GamePhase.Finished;

            // Act
            await controller.PlayRoundAsync();

            // Assert
            await _deckProvider.DidNotReceive().DrawCardsAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task PlayRoundAsync_CompletesRoundSuccessfully()
        {
            // Arrange
            _deckProvider.InitializeAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
            var drawnCards = new List<Card>
            {
                new(CardSuit.Hearts, CardValue.Two),
                new(CardSuit.Spades, CardValue.Ace)
            };
            _deckProvider.DrawCardsAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IReadOnlyList<Card>>(drawnCards));
            
            foreach (var playerController in _playerControllers)
            {
                playerController.RequestDrawAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
            }
            
            var controller = new GameController(_deckProvider, _players, _playerControllers, _gameView, _gameMode, _gameState, _logger);
            _gameMode.ConfigureNewGame(_gameState, _players);
            

            // Act
            await controller.StartGameAsync();
            await controller.PlayRoundAsync();

            // Assert
            await _deckProvider.Received(1).DrawCardsAsync(2, Arg.Any<CancellationToken>());
            _gameView.Received().ShowRoundResult(Arg.Any<int>(), Arg.Any<IReadOnlyList<Card>>(), Arg.Any<IReadOnlyList<int>>(), Arg.Any<int>());
            _gameView.Received().UpdateScores(Arg.Any<List<int>>());
            Assert.AreEqual(1, _gameState.CurrentRound);
        }

        [Test]
        public async Task PlayRoundAsync_RequestsDrawFromAllControllers()
        {
            // Arrange
            _deckProvider.InitializeAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
            var drawnCards = new List<Card>
            {
                new(CardSuit.Hearts, CardValue.Two),
                new(CardSuit.Spades, CardValue.Ace)
            };
            _deckProvider.DrawCardsAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IReadOnlyList<Card>>(drawnCards));
            
            foreach (var playerController in _playerControllers)
            {
                playerController.RequestDrawAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
            }
            
            var controller = new GameController(_deckProvider, _players, _playerControllers, _gameView, _gameMode, _gameState, _logger);
            _gameMode.ConfigureNewGame(_gameState, _players);
            

            // Act
            await controller.StartGameAsync();
            await controller.PlayRoundAsync();

            // Assert
            foreach (var playerController in _playerControllers)
            {
                await playerController.Received(1).RequestDrawAsync(Arg.Any<CancellationToken>());
            }
        }

        [Test]
        public async Task PlayRoundAsync_WhenMaxRoundsReached_EndsGame()
        {
            // Arrange
            _deckProvider.InitializeAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
            var drawnCards = new List<Card>
            {
                new(CardSuit.Hearts, CardValue.Two),
                new(CardSuit.Spades, CardValue.Ace)
            };
            _deckProvider.DrawCardsAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IReadOnlyList<Card>>(drawnCards));
            
            foreach (var playerController in _playerControllers)
            {
                playerController.RequestDrawAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
            }
            
            var controller = new GameController(_deckProvider, _players, _playerControllers, _gameView, _gameMode, _gameState, _logger);
            _gameMode.ConfigureNewGame(_gameState, _players);


            // Act
            await controller.StartGameAsync();
            _gameState.CurrentRound = 7; // One round before max
            await controller.PlayRoundAsync();

            // Assert
            Assert.AreEqual(GamePhase.Finished, _gameState.Phase);
            _gameView.Received().ShowGameOver(Arg.Any<int>());
        }

        [Test]
        public async Task PlayRoundAsync_WhenDrawFails_HandlesError()
        {
            // Arrange
            _deckProvider.InitializeAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
            _deckProvider.DrawCardsAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IReadOnlyList<Card>>(null)); // Simulate draw failure
            
            foreach (var playerController in _playerControllers)
            {
                playerController.RequestDrawAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
            }
            
            var controller = new GameController(_deckProvider, _players, _playerControllers, _gameView, _gameMode, _gameState, _logger);
            _gameMode.ConfigureNewGame(_gameState, _players);
            

            // Act
            await controller.StartGameAsync();
            await controller.PlayRoundAsync();

            // Assert
            // Should handle error gracefully without crashing
            // The method should complete without throwing
        }

        [Test]
        public async Task PlayRoundAsync_WithCancellation_ThrowsCancellationException()
        {
            // Arrange
            _deckProvider.InitializeAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
            var cts = new CancellationTokenSource();
            cts.Cancel();
            
            foreach (var playerController in _playerControllers)
            {
                playerController.RequestDrawAsync(Arg.Any<CancellationToken>())
                    .Returns(Task.FromException(new TaskCanceledException()));
            }
            
            var controller = new GameController(_deckProvider, _players, _playerControllers, _gameView, _gameMode, _gameState, _logger);
            _gameMode.ConfigureNewGame(_gameState, _players);
            

            // Act & Assert
            await controller.StartGameAsync();
            
            Assert.ThrowsAsync<TaskCanceledException>(async () =>
            {
                await controller.PlayRoundAsync(cts.Token);
            });
        }
    }
}


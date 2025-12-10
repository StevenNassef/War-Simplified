using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using NSubstitute;
using Game.Core.Abstractions;
using Game.Core.Application;
using Game.Core.Model;

namespace Game.Core.Tests.Application
{
    [TestFixture]
    public class DeckProviderServiceTests
    {
        private ICardApiClient _apiClient;
        private DeckProviderService _deckProviderService;

        [SetUp]
        public void SetUp()
        {
            _apiClient = Substitute.For<ICardApiClient>();
            _deckProviderService = new DeckProviderService(_apiClient);
        }

        [Test]
        public async Task InitializeAsync_SuccessfullyInitializesDeck()
        {
            // Arrange
            const string expectedDeckId = "test-deck-123";
            const int expectedRemaining = 52;
            _apiClient.CreateAndShuffleNewDeckAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult((expectedDeckId, expectedRemaining)));

            // Act
            await _deckProviderService.InitializeAsync();

            // Assert
            await _apiClient.Received(1).CreateAndShuffleNewDeckAsync(Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task InitializeAsync_WithCancellationToken_PassesTokenToApiClient()
        {
            // Arrange
            var cancellationToken = new CancellationTokenSource().Token;
            _apiClient.CreateAndShuffleNewDeckAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(("deck-id", 52)));

            // Act
            await _deckProviderService.InitializeAsync(cancellationToken);

            // Assert
            await _apiClient.Received(1).CreateAndShuffleNewDeckAsync(cancellationToken);
        }

        [Test]
        public async Task ReshuffleAsync_WhenDeckInitialized_SuccessfullyReshuffles()
        {
            // Arrange
            const string deckId = "test-deck-123";
            const int remainingAfterReshuffle = 52;
            _apiClient.CreateAndShuffleNewDeckAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult((deckId, 10)));
            _apiClient.ReshuffleDeckAsync(deckId, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult((deckId, remainingAfterReshuffle)));

            await _deckProviderService.InitializeAsync();

            // Act
            await _deckProviderService.ReshuffleAsync();

            // Assert
            await _apiClient.Received(1).ReshuffleDeckAsync(deckId, Arg.Any<CancellationToken>());
        }

        [Test]
        public void ReshuffleAsync_WhenDeckNotInitialized_ThrowsInvalidOperationException()
        {
            // Act and Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _deckProviderService.ReshuffleAsync();
            });
        }

        [Test]
        public async Task ReshuffleAsync_WithCancellationToken_PassesTokenToApiClient()
        {
            // Arrange
            const string deckId = "test-deck-123";
            var cancellationToken = new CancellationTokenSource().Token;
            _apiClient.CreateAndShuffleNewDeckAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult((deckId, 52)));
            _apiClient.ReshuffleDeckAsync(deckId, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult((deckId, 52)));

            await _deckProviderService.InitializeAsync();

            // Act
            await _deckProviderService.ReshuffleAsync(cancellationToken);

            // Assert
            await _apiClient.Received(1).ReshuffleDeckAsync(deckId, cancellationToken);
        }

        [Test]
        public async Task DrawCardsAsync_WhenDeckInitialized_ReturnsDrawnCards()
        {
            // Arrange
            const string deckId = "test-deck-123";
            var expectedCards = new Card[]
            {
                new(CardSuit.Hearts, CardValue.Ace),
                new(CardSuit.Spades, CardValue.King)
            };
            const int remainingAfterDraw = 50;

            _apiClient.CreateAndShuffleNewDeckAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult((deckId, 52)));
            _apiClient.DrawCardsAsync(deckId, 2, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult((expectedCards, remainingAfterDraw)));

            await _deckProviderService.InitializeAsync();

            // Act
            var result = await _deckProviderService.DrawCardsAsync(2);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(CardValue.Ace, result[0].CardValue);
            Assert.AreEqual(CardValue.King, result[1].CardValue);
            Assert.AreEqual(CardSuit.Hearts, result[0].Suit);
            Assert.AreEqual(CardSuit.Spades, result[1].Suit);
            await _apiClient.Received(1).DrawCardsAsync(deckId, 2, Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task DrawCardsAsync_WithDefaultCount_DrawsOneCard()
        {
            // Arrange
            const string deckId = "test-deck-123";
            var expectedCards = new Card[] { new(CardSuit.Hearts, CardValue.Ace) };

            _apiClient.CreateAndShuffleNewDeckAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult((deckId, 52)));
            _apiClient.DrawCardsAsync(deckId, 1, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult((expectedCards, 51)));

            await _deckProviderService.InitializeAsync();

            // Act
            var result = await _deckProviderService.DrawCardsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            await _apiClient.Received(1).DrawCardsAsync(deckId, 1, Arg.Any<CancellationToken>());
        }

        [Test]
        public void DrawCardsAsync_WithZeroCount_ThrowsArgumentOutOfRangeException()
        {
            // Act and Assert
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            {
                await _deckProviderService.DrawCardsAsync(0);
            });
        }

        [Test]
        public void DrawCardsAsync_WithNegativeCount_ThrowsArgumentOutOfRangeException()
        {
            // Act and Assert
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            {
                await _deckProviderService.DrawCardsAsync(-1);
            });
        }

        [Test]
        public async Task DrawCardsAsync_WithCancellationToken_PassesTokenToApiClient()
        {
            // Arrange
            const string deckId = "test-deck-123";
            var cancellationToken = new CancellationTokenSource().Token;
            var expectedCards = new Card[] { new(CardSuit.Hearts, CardValue.Ace) };

            _apiClient.CreateAndShuffleNewDeckAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult((deckId, 52)));
            _apiClient.DrawCardsAsync(deckId, 1, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult((expectedCards, 51)));

            await _deckProviderService.InitializeAsync();

            // Act
            await _deckProviderService.DrawCardsAsync(1, cancellationToken);

            // Assert
            await _apiClient.Received(1).DrawCardsAsync(deckId, 1, cancellationToken);
        }

        [Test]
        public async Task DrawCardsAsync_MultipleDraws_UsesSameDeckId()
        {
            // Arrange
            const string deckId = "test-deck-123";
            var firstDraw = new Card[] { new(CardSuit.Hearts, CardValue.Ace) };
            var secondDraw = new Card[] { new(CardSuit.Spades, CardValue.King) };

            _apiClient.CreateAndShuffleNewDeckAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult((deckId, 52)));
            _apiClient.DrawCardsAsync(deckId, 1, Arg.Any<CancellationToken>())
                .Returns(
                    Task.FromResult((firstDraw, 51)),
                    Task.FromResult((secondDraw, 50)));

            await _deckProviderService.InitializeAsync();

            // Act
            var firstResult = await _deckProviderService.DrawCardsAsync(1);
            var secondResult = await _deckProviderService.DrawCardsAsync(1);

            // Assert
            Assert.AreEqual(1, firstResult.Count);
            Assert.AreEqual(1, secondResult.Count);
            await _apiClient.Received(2).DrawCardsAsync(deckId, 1, Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task DrawCardsAsync_WithLargeCount_DrawsMultipleCards()
        {
            // Arrange
            const string deckId = "test-deck-123";
            const int drawCount = 5;
            var expectedCards = new Card[drawCount];
            for (var i = 0; i < drawCount; i++)
            {
                expectedCards[i] = new Card(CardSuit.Hearts, CardValue.Two + i);
            }

            _apiClient.CreateAndShuffleNewDeckAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult((deckId, 52)));
            _apiClient.DrawCardsAsync(deckId, drawCount, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult((expectedCards, 47)));

            await _deckProviderService.InitializeAsync();

            // Act
            var result = await _deckProviderService.DrawCardsAsync(drawCount);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(drawCount, result.Count);
            for (var i = 0; i < result.Count; i++)
            {
                Assert.AreEqual(CardSuit.Hearts, result[i].Suit);
                Assert.AreEqual(CardValue.Two + i, result[i].CardValue);
            }
            await _apiClient.Received(1).DrawCardsAsync(deckId, drawCount, Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task InitializeAsync_ThenReshuffle_ThenDraw_WorksCorrectly()
        {
            // Arrange
            const string deckId = "test-deck-123";
            var drawnCards = new Card[] { new(CardSuit.Hearts, CardValue.Ace) };

            _apiClient.CreateAndShuffleNewDeckAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult((deckId, 52)));
            _apiClient.ReshuffleDeckAsync(deckId, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult((deckId, 52)));
            _apiClient.DrawCardsAsync(deckId, 1, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult((drawnCards, 51)));

            // Act
            await _deckProviderService.InitializeAsync();
            await _deckProviderService.ReshuffleAsync();
            var result = await _deckProviderService.DrawCardsAsync(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            await _apiClient.Received(1).CreateAndShuffleNewDeckAsync(Arg.Any<CancellationToken>());
            await _apiClient.Received(1).ReshuffleDeckAsync(deckId, Arg.Any<CancellationToken>());
            await _apiClient.Received(1).DrawCardsAsync(deckId, 1, Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task DrawCardsAsync_WhenCancelled_PropagatesCancellation()
        {
            // Arrange
            const string deckId = "test-deck-123";
            var cts = new CancellationTokenSource();
            cts.Cancel();

            _apiClient.CreateAndShuffleNewDeckAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult((deckId, 52)));
            _apiClient.DrawCardsAsync(deckId, 1, Arg.Any<CancellationToken>())
                .Returns(Task.FromCanceled<(Card[], int)>(cts.Token));

            await _deckProviderService.InitializeAsync();

            // Act & Assert
            Assert.ThrowsAsync<TaskCanceledException>(async () =>
            {
                await _deckProviderService.DrawCardsAsync(1, cts.Token);
            });
        }
    }
}


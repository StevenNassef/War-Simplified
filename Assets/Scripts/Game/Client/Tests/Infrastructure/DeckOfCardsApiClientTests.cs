using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Game.Client.Infrastructure;
using Game.Core.Model;
using UnityEngine;

namespace Game.Client.Tests.Infrastructure
{
    [TestFixture]
    public class DeckOfCardsApiClientTests
    {
        private DeckOfCardsApiClient _apiClient;

        [SetUp]
        public void SetUp()
        {
            _apiClient = new DeckOfCardsApiClient();
        }

        #region CreateAndShuffleNewDeckAsync Tests

        [Test]
        public async Task CreateAndShuffleNewDeckAsync_Success_ReturnsDeckIdAndRemaining()
        {
            // Act
            var (deckId, remaining) = await _apiClient.CreateAndShuffleNewDeckAsync();

            // Assert
            Assert.IsNotNull(deckId);
            Assert.IsNotEmpty(deckId);
            Assert.AreEqual(52, remaining);
        }

        [Test]
        public async Task CreateAndShuffleNewDeckAsync_WithCancellationToken_CompletesSuccessfully()
        {
            // Arrange
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            var (deckId, remaining) = await _apiClient.CreateAndShuffleNewDeckAsync(cancellationToken);

            // Assert
            Assert.IsNotNull(deckId);
            Assert.IsNotEmpty(deckId);
            Assert.AreEqual(52, remaining);
        }

        [Test]
        public async Task CreateAndShuffleNewDeckAsync_WhenCancelled_ThrowsTaskCanceledException()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act and Assert
            Assert.ThrowsAsync<TaskCanceledException>(async () =>
            {
                await _apiClient.CreateAndShuffleNewDeckAsync(cts.Token);
            });
        }

        #endregion

        #region ReshuffleDeckAsync Tests

        [Test]
        public async Task ReshuffleDeckAsync_WithValidDeckId_SuccessfullyReshuffles()
        {
            // Arrange
            var (deckId, _) = await _apiClient.CreateAndShuffleNewDeckAsync();

            // Act
            var (reshuffledDeckId, remaining) = await _apiClient.ReshuffleDeckAsync(deckId);

            // Assert
            Assert.AreEqual(deckId, reshuffledDeckId);
            Assert.AreEqual(52, remaining);
        }

        // [Test]
        // public void ReshuffleDeckAsync_WithNullDeckId_ThrowsArgumentException()
        // {
        //     // Act and Assert
        //     Assert.ThrowsAsync<ArgumentException>(async () => { await _apiClient.ReshuffleDeckAsync(null); });
        // }
        //
        // [Test]
        // public void ReshuffleDeckAsync_WithEmptyDeckId_ThrowsArgumentException()
        // {
        //     // Act and Assert
        //     Assert.ThrowsAsync<ArgumentException>(async () => { await _apiClient.ReshuffleDeckAsync(string.Empty); });
        // }
        //
        // [Test]
        // public void ReshuffleDeckAsync_WithWhitespaceDeckId_ThrowsArgumentException()
        // {
        //     // Act and Assert
        //     Assert.ThrowsAsync<ArgumentException>(async () => { await _apiClient.ReshuffleDeckAsync("   "); });
        // }
        //
        // [Test]
        // public async Task ReshuffleDeckAsync_WithInvalidDeckId_ThrowsException()
        // {
        //     // Arrange
        //     const string invalidDeckId = "invalid-deck-id-12345";
        //
        //     // Act and Assert
        //     Assert.ThrowsAsync<Exception>(async () => { await _apiClient.ReshuffleDeckAsync(invalidDeckId); });
        // }

        [Test]
        public async Task ReshuffleDeckAsync_WithCancellationToken_CompletesSuccessfully()
        {
            // Arrange
            var (deckId, _) = await _apiClient.CreateAndShuffleNewDeckAsync();
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            var (reshuffledDeckId, remaining) = await _apiClient.ReshuffleDeckAsync(deckId, cancellationToken);

            // Assert
            Assert.AreEqual(deckId, reshuffledDeckId);
            Assert.AreEqual(52, remaining);
        }

        [Test]
        public async Task ReshuffleDeckAsync_WhenCancelled_ThrowsTaskCanceledException()
        {
            // Arrange
            var (deckId, _) = await _apiClient.CreateAndShuffleNewDeckAsync();
            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act and Assert
            Assert.ThrowsAsync<TaskCanceledException>(async () =>
            {
                await _apiClient.ReshuffleDeckAsync(deckId, cts.Token);
            });
        }

        #endregion

        #region DrawCardsAsync Tests

        [Test]
        public async Task DrawCardsAsync_WithValidDeckIdAndCount_ReturnsCards()
        {
            // Arrange
            var (deckId, _) = await _apiClient.CreateAndShuffleNewDeckAsync();
            const int count = 2;

            // Act
            var (cards, remaining) = await _apiClient.DrawCardsAsync(deckId, count);

            // Assert
            Assert.IsNotNull(cards);
            Assert.AreEqual(count, cards.Length);
            Assert.AreEqual(52 - count, remaining);
        }

        [Test]
        public async Task DrawCardsAsync_WithCountOne_ReturnsSingleCard()
        {
            // Arrange
            var (deckId, _) = await _apiClient.CreateAndShuffleNewDeckAsync();

            // Act
            var (cards, remaining) = await _apiClient.DrawCardsAsync(deckId, 1);

            // Assert
            Assert.IsNotNull(cards);
            Assert.AreEqual(1, cards.Length);
            Assert.AreEqual(51, remaining);
        }

        [Test]
        public async Task DrawCardsAsync_WithLargeCount_ReturnsMultipleCards()
        {
            // Arrange
            var (deckId, _) = await _apiClient.CreateAndShuffleNewDeckAsync();
            const int count = 5;

            // Act
            var (cards, remaining) = await _apiClient.DrawCardsAsync(deckId, count);

            // Assert
            Assert.IsNotNull(cards);
            Assert.AreEqual(count, cards.Length);
            Assert.AreEqual(52 - count, remaining);
        }

        [Test]
        public async Task DrawCardsAsync_MultipleDraws_DecrementsRemainingCorrectly()
        {
            // Arrange
            var (deckId, initialRemaining) = await _apiClient.CreateAndShuffleNewDeckAsync();

            // Act
            var (firstDraw, remainingAfterFirst) = await _apiClient.DrawCardsAsync(deckId, 3);
            var (secondDraw, remainingAfterSecond) = await _apiClient.DrawCardsAsync(deckId, 2);

            // Assert
            Assert.AreEqual(3, firstDraw.Length);
            Assert.AreEqual(2, secondDraw.Length);
            Assert.AreEqual(initialRemaining - 3, remainingAfterFirst);
            Assert.AreEqual(remainingAfterFirst - 2, remainingAfterSecond);
        }

        // [Test]
        // public void DrawCardsAsync_WithNullDeckId_ThrowsArgumentException()
        // {
        //     // Act and Assert
        //     Assert.ThrowsAsync<ArgumentException>(async () => { await _apiClient.DrawCardsAsync(null, 1); });
        // }

        // [Test]
        // public void DrawCardsAsync_WithEmptyDeckId_ThrowsArgumentException()
        // {
        //     // Act and Assert
        //     Assert.ThrowsAsync<ArgumentException>(async () => { await _apiClient.DrawCardsAsync(string.Empty, 1); });
        // }

        // [Test]
        // public async Task DrawCardsAsync_WithZeroCount_ThrowsArgumentOutOfRangeException()
        // {
        //     // Arrange
        //     var (deckId, _) = await _apiClient.CreateAndShuffleNewDeckAsync();
        //
        //     // Act and Assert
        //     Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
        //     {
        //         await _apiClient.DrawCardsAsync(deckId, 0);
        //     });
        // }

        // [Test]
        // public async Task DrawCardsAsync_WithNegativeCount_ThrowsArgumentOutOfRangeException()
        // {
        //     // Arrange
        //     var (deckId, _) = await _apiClient.CreateAndShuffleNewDeckAsync();
        //
        //     // Act and Assert
        //     Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
        //     {
        //         await _apiClient.DrawCardsAsync(deckId, -1);
        //     });
        // }

        // [Test]
        // public void DrawCardsAsync_WithInvalidDeckId_ThrowsException()
        // {
        //     // Arrange
        //     const string invalidDeckId = "invalid-deck-id-12345";
        //
        //     // Act and Assert
        //     Assert.ThrowsAsync<Exception>(async () => { await _apiClient.DrawCardsAsync(invalidDeckId, 1); });
        // }

        [Test]
        public async Task DrawCardsAsync_WithCancellationToken_CompletesSuccessfully()
        {
            // Arrange
            var (deckId, _) = await _apiClient.CreateAndShuffleNewDeckAsync();
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            var (cards, remaining) = await _apiClient.DrawCardsAsync(deckId, 1, cancellationToken);

            // Assert
            Assert.IsNotNull(cards);
            Assert.AreEqual(1, cards.Length);
            Assert.AreEqual(51, remaining);
        }

        [Test]
        public async Task DrawCardsAsync_WhenCancelled_ThrowsTaskCanceledException()
        {
            // Arrange
            var (deckId, _) = await _apiClient.CreateAndShuffleNewDeckAsync();
            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act and Assert
            Assert.ThrowsAsync<TaskCanceledException>(async () =>
            {
                await _apiClient.DrawCardsAsync(deckId, 1, cts.Token);
            });
        }

        #endregion

        #region Card Conversion Tests (Indirect)

        [Test]
        public async Task DrawCardsAsync_ConvertsAllSuitsCorrectly()
        {
            // Arrange
            var (deckId, _) = await _apiClient.CreateAndShuffleNewDeckAsync();

            // Act
            var (cards, _) = await _apiClient.DrawCardsAsync(deckId, 52);

            // Assert
            var suits = cards.Select(c => c.Suit).Distinct().ToArray();
            Assert.Contains(CardSuit.Spades, suits);
            Assert.Contains(CardSuit.Hearts, suits);
            Assert.Contains(CardSuit.Diamonds, suits);
            Assert.Contains(CardSuit.Clubs, suits);
        }

        [Test]
        public async Task DrawCardsAsync_ConvertsAllCardValuesCorrectly()
        {
            // Arrange
            var (deckId, _) = await _apiClient.CreateAndShuffleNewDeckAsync();

            // AcT
            var (cards, _) = await _apiClient.DrawCardsAsync(deckId, 52);

            // Assert
            var values = cards.Select(c => c.CardValue).Distinct().ToArray();
            Assert.Contains(CardValue.Two, values);
            Assert.Contains(CardValue.Three, values);
            Assert.Contains(CardValue.Four, values);
            Assert.Contains(CardValue.Five, values);
            Assert.Contains(CardValue.Six, values);
            Assert.Contains(CardValue.Seven, values);
            Assert.Contains(CardValue.Eight, values);
            Assert.Contains(CardValue.Nine, values);
            Assert.Contains(CardValue.Ten, values);
            Assert.Contains(CardValue.Jack, values);
            Assert.Contains(CardValue.Queen, values);
            Assert.Contains(CardValue.King, values);
            Assert.Contains(CardValue.Ace, values);
        }

        [Test]
        public async Task DrawCardsAsync_ConvertsNumericValuesCorrectly()
        {
            // Arrange
            var (deckId, _) = await _apiClient.CreateAndShuffleNewDeckAsync();

            // Act
            var (cards, _) = await _apiClient.DrawCardsAsync(deckId, 52);

            // Assert
            var numericCards = cards.Where(c => c.CardValue is >= CardValue.Two and <= CardValue.Ten).ToArray();
            Assert.IsTrue(numericCards.Length > 0, "Should have numeric cards");
        }

        [Test]
        public async Task DrawCardsAsync_ConvertsFaceCardsCorrectly()
        {
            // Arrange
            var (deckId, _) = await _apiClient.CreateAndShuffleNewDeckAsync();

            // Act
            var (cards, _) = await _apiClient.DrawCardsAsync(deckId, 52);

            // Assert
            var faceCards = cards.Where(c =>
                c.CardValue is CardValue.Jack or CardValue.Queen or CardValue.King or CardValue.Ace).ToArray();
            Assert.IsTrue(faceCards.Length >= 4, "Should have at least one of each face card");
        }

        #endregion

        #region Integration Flow Tests

        [Test]
        public async Task FullWorkflow_CreateShuffleDrawReshuffleDraw_WorksCorrectly()
        {
            // Arrange and Act
            var (deckId1, remaining1) = await _apiClient.CreateAndShuffleNewDeckAsync();
            var (cards1, remaining2) = await _apiClient.DrawCardsAsync(deckId1, 5);
            var (deckId2, remaining3) = await _apiClient.ReshuffleDeckAsync(deckId1);
            var (cards2, remaining4) = await _apiClient.DrawCardsAsync(deckId2, 3);

            // Assert
            Assert.AreEqual(52, remaining1);
            Assert.AreEqual(5, cards1.Length);
            Assert.AreEqual(47, remaining2);
            Assert.AreEqual(deckId1, deckId2);
            Assert.AreEqual(52, remaining3);
            Assert.AreEqual(3, cards2.Length);
            Assert.AreEqual(49, remaining4);
        }

        [Test]
        public async Task MultipleDecks_CanCreateAndUseSeparately()
        {
            // Arrange and Act
            var (deckId1, remaining1) = await _apiClient.CreateAndShuffleNewDeckAsync();
            var (deckId2, remaining2) = await _apiClient.CreateAndShuffleNewDeckAsync();

            var (cards1, _) = await _apiClient.DrawCardsAsync(deckId1, 3);
            var (cards2, _) = await _apiClient.DrawCardsAsync(deckId2, 3);

            // Assert
            Assert.AreNotEqual(deckId1, deckId2);
            Assert.AreEqual(52, remaining1);
            Assert.AreEqual(52, remaining2);
            Assert.AreEqual(3, cards1.Length);
            Assert.AreEqual(3, cards2.Length);
        }

        #endregion

        #region Edge Cases

        [Test]
        public async Task DrawCardsAsync_WithMaximumCount_DrawsAllCards()
        {
            // Arrange
            var (deckId, _) = await _apiClient.CreateAndShuffleNewDeckAsync();

            // Act
            var (cards, remaining) = await _apiClient.DrawCardsAsync(deckId, 52);

            // Assert
            Assert.AreEqual(52, cards.Length);
            Assert.AreEqual(0, remaining);
        }

        // [Test]
        // public async Task DrawCardsAsync_AfterDrawingAllCards_ThrowsException()
        // {
        //     // Arrange
        //     var (deckId, _) = await _apiClient.CreateAndShuffleNewDeckAsync();
        //     
        //     await _apiClient.DrawCardsAsync(deckId, 52);
        //     
        //     // Act and Assert - Add timeout to prevent hanging
        //
        //     Assert.ThrowsAsync<Exception>(async () => await _apiClient.DrawCardsAsync(deckId, 1));
        // }

        #endregion
    }
}
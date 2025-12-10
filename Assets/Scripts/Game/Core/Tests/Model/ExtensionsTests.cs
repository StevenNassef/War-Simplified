using System.Collections.Generic;
using NUnit.Framework;
using Game.Core.Model;

namespace Game.Core.Tests.Model
{
    [TestFixture]
    public class ExtensionsTests
    {
        [Test]
        public void GetUniqueHighestItemByIndex_WithUniqueHighest_ReturnsCorrectIndex()
        {
            // Arrange
            var items = new List<int> { 1, 5, 3, 2 };

            // Act
            var result = items.GetUniqueHighestItemByIndex();

            // Assert
            Assert.AreEqual(1, result); // Index of 5
        }

        [Test]
        public void GetUniqueHighestItemByIndex_WithTies_ReturnsNegativeOne()
        {
            // Arrange
            var items = new List<int> { 1, 5, 5, 2 };

            // Act
            var result = items.GetUniqueHighestItemByIndex();

            // Assert
            Assert.AreEqual(-1, result);
        }

        [Test]
        public void GetUniqueHighestItemByIndex_WithEmptyCollection_ReturnsNegativeOne()
        {
            // Arrange
            var items = new List<int>();

            // Act
            var result = items.GetUniqueHighestItemByIndex();

            // Assert
            Assert.AreEqual(-1, result);
        }

        [Test]
        public void GetUniqueHighestItemByIndex_WithNullCollection_ReturnsNegativeOne()
        {
            // Arrange
            IReadOnlyList<int> items = null;

            // Act
            var result = items.GetUniqueHighestItemByIndex();

            // Assert
            Assert.AreEqual(-1, result);
        }

        [Test]
        public void GetUniqueHighestItemByIndex_WithSingleItem_ReturnsZero()
        {
            // Arrange
            var items = new List<int> { 42 };

            // Act
            var result = items.GetUniqueHighestItemByIndex();

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void GetUniqueHighestItemByIndex_WithOutParameter_ReturnsHighestItem()
        {
            // Arrange
            var items = new List<int> { 1, 5, 3, 2 };

            // Act
            var index = items.GetUniqueHighestItemByIndex(out int highestItem);

            // Assert
            Assert.AreEqual(1, index);
            Assert.AreEqual(5, highestItem);
        }

        [Test]
        public void GetUniqueHighestItemByIndex_WithTies_OutParameterIsDefault()
        {
            // Arrange
            var items = new List<int> { 1, 5, 5, 2 };

            // Act
            var index = items.GetUniqueHighestItemByIndex(out int highestItem);

            // Assert
            Assert.AreEqual(-1, index);
            Assert.AreEqual(0, highestItem); // default(int)
        }

        [Test]
        public void GetUniqueHighestItemByIndex_WithComparer_UniqueHighest_ReturnsCorrectIndex()
        {
            // Arrange
            var cards = new List<Card>
            {
                new(CardSuit.Hearts, CardValue.Two),
                new(CardSuit.Spades, CardValue.Ace),
                new(CardSuit.Diamonds, CardValue.King)
            };
            var comparer = Card.ValueOnlyComparerInstance;

            // Act
            var result = cards.GetUniqueHighestItemByIndex(comparer);

            // Assert
            Assert.AreEqual(1, result); // Index of Ace
        }

        [Test]
        public void GetUniqueHighestItemByIndex_WithComparer_Ties_ReturnsNegativeOne()
        {
            // Arrange
            var cards = new List<Card>
            {
                new(CardSuit.Hearts, CardValue.Ace),
                new(CardSuit.Spades, CardValue.Ace),
                new(CardSuit.Diamonds, CardValue.King)
            };
            var comparer = Card.ValueOnlyComparerInstance;

            // Act
            var result = cards.GetUniqueHighestItemByIndex(comparer);

            // Assert
            Assert.AreEqual(-1, result);
        }

        [Test]
        public void GetUniqueHighestItemByIndex_WithComparer_OutParameter_ReturnsHighestItem()
        {
            // Arrange
            var cards = new List<Card>
            {
                new(CardSuit.Hearts, CardValue.Two),
                new(CardSuit.Spades, CardValue.Ace),
                new(CardSuit.Diamonds, CardValue.King)
            };
            var comparer = Card.ValueOnlyComparerInstance;

            // Act
            var index = cards.GetUniqueHighestItemByIndex(comparer, out Card highestCard);

            // Assert
            Assert.AreEqual(1, index);
            Assert.AreEqual(CardValue.Ace, highestCard.CardValue);
            Assert.AreEqual(CardSuit.Spades, highestCard.Suit);
        }

        [Test]
        public void GetUniqueHighestItemByIndex_AllSameValue_ReturnsNegativeOne()
        {
            // Arrange
            var items = new List<int> { 5, 5, 5, 5 };

            // Act
            var result = items.GetUniqueHighestItemByIndex();

            // Assert
            Assert.AreEqual(-1, result);
        }

        [Test]
        public void GetUniqueHighestItemByIndex_FirstItemHighest_ReturnsZero()
        {
            // Arrange
            var items = new List<int> { 10, 5, 3, 2 };

            // Act
            var result = items.GetUniqueHighestItemByIndex();

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void GetUniqueHighestItemByIndex_LastItemHighest_ReturnsLastIndex()
        {
            // Arrange
            var items = new List<int> { 1, 2, 3, 10 };

            // Act
            var result = items.GetUniqueHighestItemByIndex();

            // Assert
            Assert.AreEqual(3, result);
        }
    }
}


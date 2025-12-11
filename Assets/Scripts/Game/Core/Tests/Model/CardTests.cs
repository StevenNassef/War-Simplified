using NUnit.Framework;
using Game.Core.Model;

namespace Game.Core.Tests.Model
{
    [TestFixture]
    public class CardTests
    {
        [Test]
        public void Card_Creation_SetsPropertiesCorrectly()
        {
            // Arrange & Act
            var card = new Card(CardSuit.Hearts, CardValue.Ace);

            // Assert
            Assert.AreEqual(CardSuit.Hearts, card.Suit);
            Assert.AreEqual(CardValue.Ace, card.CardValue);
        }

        [Test]
        public void ValueOnlyComparer_AceGreaterThanKing_ReturnsPositive()
        {
            // Arrange
            var ace = new Card(CardSuit.Hearts, CardValue.Ace);
            var king = new Card(CardSuit.Spades, CardValue.King);
            var comparer = Card.ValueOnlyComparerInstance;

            // Act
            var result = comparer.Compare(ace, king);

            // Assert
            Assert.Greater(result, 0);
        }

        [Test]
        public void ValueOnlyComparer_KingLessThanAce_ReturnsNegative()
        {
            // Arrange
            var king = new Card(CardSuit.Hearts, CardValue.King);
            var ace = new Card(CardSuit.Spades, CardValue.Ace);
            var comparer = Card.ValueOnlyComparerInstance;

            // Act
            var result = comparer.Compare(king, ace);

            // Assert
            Assert.Less(result, 0);
        }

        [Test]
        public void ValueOnlyComparer_SameValue_ReturnsZero()
        {
            // Arrange
            var card1 = new Card(CardSuit.Hearts, CardValue.Ace);
            var card2 = new Card(CardSuit.Spades, CardValue.Ace);
            var comparer = Card.ValueOnlyComparerInstance;

            // Act
            var result = comparer.Compare(card1, card2);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void ValueOnlyComparer_TwoLessThanThree_ReturnsNegative()
        {
            // Arrange
            var two = new Card(CardSuit.Hearts, CardValue.Two);
            var three = new Card(CardSuit.Spades, CardValue.Three);
            var comparer = Card.ValueOnlyComparerInstance;

            // Act
            var result = comparer.Compare(two, three);

            // Assert
            Assert.Less(result, 0);
        }

        [Test]
        public void SuitOnlyComparer_SpadesLessThanHearts_ReturnsNegative()
        {
            // Arrange
            var spades = new Card(CardSuit.Spades, CardValue.Ace);
            var hearts = new Card(CardSuit.Hearts, CardValue.Ace);
            var comparer = Card.SuitOnlyComparerInstance;

            // Act
            var result = comparer.Compare(spades, hearts);

            // Assert
            Assert.Less(result, 0);
        }

        [Test]
        public void SuitOnlyComparer_HeartsGreaterThanSpades_ReturnsPositive()
        {
            // Arrange
            var hearts = new Card(CardSuit.Hearts, CardValue.Ace);
            var spades = new Card(CardSuit.Spades, CardValue.Ace);
            var comparer = Card.SuitOnlyComparerInstance;

            // Act
            var result = comparer.Compare(hearts, spades);

            // Assert
            Assert.Greater(result, 0);
        }

        [Test]
        public void SuitOnlyComparer_SameSuit_ReturnsZero()
        {
            // Arrange
            var card1 = new Card(CardSuit.Hearts, CardValue.Ace);
            var card2 = new Card(CardSuit.Hearts, CardValue.King);
            var comparer = Card.SuitOnlyComparerInstance;

            // Act
            var result = comparer.Compare(card1, card2);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void CardValue_EnumValues_AreCorrect()
        {
            // Assert
            Assert.AreEqual(2, (int)CardValue.Two);
            Assert.AreEqual(14, (int)CardValue.Ace);
            Assert.AreEqual(11, (int)CardValue.Jack);
            Assert.AreEqual(12, (int)CardValue.Queen);
            Assert.AreEqual(13, (int)CardValue.King);
        }

        [Test]
        public void CardSuit_EnumValues_AreDefined()
        {
            // Assert - Verify enum values exist
            Assert.IsTrue(System.Enum.IsDefined(typeof(CardSuit), CardSuit.Spades));
            Assert.IsTrue(System.Enum.IsDefined(typeof(CardSuit), CardSuit.Hearts));
            Assert.IsTrue(System.Enum.IsDefined(typeof(CardSuit), CardSuit.Diamonds));
            Assert.IsTrue(System.Enum.IsDefined(typeof(CardSuit), CardSuit.Clubs));
        }
    }
}


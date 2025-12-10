using System.Collections.Generic;

namespace Game.Core.Model
{
    public readonly struct Card
    {
        public Card(CardSuit suit, CardValue cardValue)
        {
            Suit = suit;
            CardValue = cardValue;
        }

        public CardValue CardValue { get; }
        public CardSuit Suit { get; }

        #region Helpers

        public static ValueOnlyComparer ValueOnlyComparerInstance { get; } = new();

        public static SuitOnlyComparer SuitOnlyComparerInstance { get; } = new();

        public class ValueOnlyComparer : IComparer<Card>
        {
            public int Compare(Card x, Card y)
            {
                return x.CardValue.CompareTo(y.CardValue);
            }
        }

        public class SuitOnlyComparer : IComparer<Card>
        {
            public int Compare(Card x, Card y)
            {
                return x.Suit.CompareTo(y.Suit);
            }
        }

        #endregion
    }

    public enum CardSuit
    {
        Spades,
        Hearts,
        Diamonds,
        Clubs
    }

    public enum CardValue
    {
        Ace = 1,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
    }
}
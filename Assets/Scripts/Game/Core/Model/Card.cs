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
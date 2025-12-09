namespace Game.Model
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
}
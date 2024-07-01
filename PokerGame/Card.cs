namespace PokerGame
{
    public class Card
    {
        public Card(Suit suit, string number, int value) 
        {
            Suit = suit;
            Number = number;
            Value = value;
        }
        public Suit Suit { get; set; }
        public string Number { get; set; } 
        public int Value { get; set; }
        public int Position { get; set; }
    }
}

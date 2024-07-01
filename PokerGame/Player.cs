
namespace PokerGame
{
    public class Player
    {
        public Player(string name, int currentCash, bool isDealer, bool isComputer, int position)
        {
            Name = name;
            CurrentCash = currentCash;
            IsDealer = isDealer;
            IsComputer = isComputer;
            Position = position;
            CardsInHand = new List<Card>(2);
            HandValue = 0;
            CurrentBet = 0;
        }

        public string Name { get; set; }
        public int CurrentCash { get; set; }
        public bool IsComputer { get; set; }
        public bool IsDealer { get; set; }
        public int Position { get; set; }
        public int CurrentBet { get; set; }
        public PokerAction CurrentAction { get; set; }
        public List<Card> CardsInHand { get; set; }
        public decimal HandValue { get; set; }

    }
}

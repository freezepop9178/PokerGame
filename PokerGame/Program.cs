using PokerGame;
using System.Numerics;

var exitGame = false;
var cardList = new List<Card>();
var computerNames = new List<string>()
{
    "Jimmy",
    "Jackie",
    "Betsy",
    "Flopsie",
    "Danger",
    "Banger",
    "Flintlock",
    "Rapscalion",
    "Jehovah",
    "Krampus",
    "Bananas",
};
var playerList = new List<Player>();

SetupCards();

var numberOfPlayers = GetNumberFromPlayer("How many people are going to play?");
SetupPlayers(numberOfPlayers);
var numberOfComputers = GetNumberFromPlayer("How many computers are going to play?");
SetupComputers(numberOfComputers);

while (true)
{
    TypewriterEffect("What do you want to do?");
    var command = Console.ReadLine();
    switch (command?.ToLower())
    {
        case "shuffle":
            ShuffleCards();
            TypewriterEffect("Cards have been shuffled");
            break;
        case "print":
        case "print cards":
            PrintAllCards();
            break;
        case "show":
        case "show cards":
            ShowAllPlayersCards();
            break;
        case "deal":
        case "deal cards":
        case "deal out cards":
            DealOutCards();
            break;
        case "play":
        case "play game":
            PlayGame();
            break;
        case "break":
        case "exit":
        case "exit game":
            exitGame = true;
            break;
    }

    if (command == "break" || exitGame)
    {
        TypewriterEffect("Ending Game...");
        break;
    }
}

void ShowAllPlayersCards()
{
    foreach (var player in playerList)
    {
        ShowPlayerCards(player);
    }
}
void ShowPlayerCards(Player player)
{
    TypewriterEffect($"{player.Name} has ");
    foreach (var card in player.CardsInHand)
    {
        TypewriterEffect($"{card.Number} of {card.Suit}");
    }
}

void PlayGame()
{
    TypewriterEffect("Shuffling........", 100);
    ShuffleCards();
    ShufflePlayers();
    TypewriterEffect("Dealing Cards....", 100);
    DealOutCards();
    TypewriterEffect("Starting Game....", 100);
    var round = 0;
    var blindAmt = 10;
    var currentBet = 0;
    var cardsOnTable = new List<Card>();
    while (true)
    {
        playerList[playerList.Count - 1].CurrentBet = blindAmt;
        playerList[playerList.Count - 2].CurrentBet = blindAmt / 2;

        foreach (var player in playerList)
        {
            if (player.CurrentAction == PokerAction.Fold) continue;

            if (player.IsComputer)
            {
                player.HandValue = CalculateHandValue(player.CardsInHand, cardsOnTable);
                var playerAction = GetPokerAction(player.HandValue, player.CurrentCash, currentBet);
                player.CurrentAction = playerAction;
                TypewriterEffect($"{player.Name} is going to {playerAction}");
            }
            else
            {
                ProcessPlayerAction(player, currentBet);
            }
        }

        if (exitGame)
        {
            break;
        }

        round++;
    }
}

void ProcessPlayerAction(Player player, int highestBid)
{
    TypewriterEffect($"What would {player.Name} like to do?");
    var command = Console.ReadLine();
    switch (command?.ToLower())
    {
        case "show":
        case "show cards":
            ShowPlayerCards(player);
            ProcessPlayerAction(player, highestBid);
            break;
        case "break":
        case "exit":
        case "exit game":
            exitGame = true;
            break;
        case "fold":
            player.CurrentAction = PokerAction.Fold;
            break;
        case "bet":
            var currentBet = GetNumberFromPlayer("How much would you like to bet?", maximumAmt: player.CurrentCash);
            player.CurrentBet = currentBet;
            TypewriterEffect("Thank you!");
            break;
        case "raise":
            var raiseAmount = GetNumberFromPlayer("How much would you like to raise?", highestBid, player.CurrentCash);
            player.CurrentBet = raiseAmount;
            TypewriterEffect("Thank you!");
            break;
    }
}

PokerAction GetPokerAction(decimal handValue, int playerCash, int currentBet)
{
    if (handValue == 100)
    {
        return PokerAction.AllIn;
    }
    if (handValue > 50 && currentBet != 0)
    {
        return PokerAction.Raise;
    }
    if (handValue > 20 && handValue <= 50 && currentBet != 0)
    {
        return PokerAction.Call;
    }
    if (handValue <= 20 && currentBet != 0)
    {
        return PokerAction.Fold;
    }
    return PokerAction.Fold;
}

void ShufflePlayers()
{
    var positionsToExclude = new List<int>();
    foreach (var player in playerList)
    {
        player.Position = GetPosition(positionsToExclude, playerList.Count + 1);
    }
    playerList = playerList.OrderBy(pl => pl.Position).ToList();
}

void ShuffleCards()
{
    var positionsToExclude = new List<int>();
    foreach (var card in cardList)
    {
        card.Position = GetPosition(positionsToExclude, cardList.Count + 1);
    }
    cardList = cardList.OrderBy(cl => cl.Position).ToList();
}

void DealOutCards()
{
    foreach (var player in playerList)
    {
        if (player.CardsInHand.Any())
        {
            TypewriterEffect($"{player.Name} already has cards!");
            continue;
        }
        for (var i = 0; i < 2; i++)
        {
            player.CardsInHand.Add(cardList.First());
            cardList.Remove(cardList.First());
        }
        //player.HandValue = CalculateHandValue(player.CardsInHand);
    }
}

decimal CalculateHandValue(List<Card> cardsInHand, List<Card> cardsOnTable)
{
    var handValue = (decimal)0;
    foreach (var card in cardsInHand)
    {
        handValue += card.Value;
    }
    foreach (var card in cardsOnTable)
    {
        var loweredValue = card.Value / 10;
        handValue += Math.Floor((decimal)loweredValue);
    }
    //Royal Flush
    //Straight Flush
    //Four of a kind
    //Full House
    //Flush
    //Straight
    //Three of a kind
    //Two Pair
    //One Pair
    //High Card
    return handValue;
}

void SetupPlayers(int numberOfPlayers)
{
    for (int i = 0; i < numberOfPlayers; i++)
    {
        var playerName = GetStringFromPlayer($"What is Player Number {i + 1}'s Name?");
        var currentCash = GetNumberFromPlayer($"Awesome! How much money does {playerName} have?");
        playerList.Add(new Player(playerName, currentCash, false, false, 0));
    }
}

void SetupComputers(int numberOfComputers)
{
    for (int i = 0; i < numberOfComputers; i++)
    {
        var randomNum = new Random().Next(1, 11);
        playerList.Add(new Player(computerNames[randomNum], randomNum * 1000, false, true, 0));
    }
}

int GetNumberFromPlayer(string message, int minimumAmt = 0, int maximumAmt = int.MaxValue)
{
    TypewriterEffect(message);
    var numString = Console.ReadLine();
    var isInt = int.TryParse(numString, out int numAmount);
    if (!isInt || numAmount <= minimumAmt || numAmount > maximumAmt)
    {
        TypewriterEffect("Try again!");
        return GetNumberFromPlayer(message);
    }
    return numAmount;
}

string GetStringFromPlayer(string message)
{
    TypewriterEffect(message);
    var playerName = Console.ReadLine();
    if (playerName == null)
    {
        TypewriterEffect("Try again!");
        return GetStringFromPlayer(message);
    }
    return playerName;
}

bool GetBoolFromPlayer(string message)
{
    TypewriterEffect(message);
    var playerAnswer = Console.ReadLine();
    switch (playerAnswer?.ToLower())
    {
        case "yes":
        case "true":
            return true;
        case "no":
        case "false":
            return false;
    }
    TypewriterEffect("Try again!");
    return GetBoolFromPlayer(message);
}

void SetupCards()
{
    var positionsToExclude = new List<int>();
    foreach (var suit in Enum.GetValues(typeof(Suit)))
    {
        for (int i = 1; i < 14; i++)
        {
            var cardNumber = i.ToString();
            var cardValue = i;
            switch (i)
            {
                case 1:
                    cardValue = 14;
                    cardNumber = "Ace";
                    break;
                case 11:
                    cardNumber = "Jack";
                    break;
                case 12:
                    cardNumber = "Queen";
                    break;
                case 13:
                    cardNumber = "King";
                    break;

            }
            cardList.Add(new Card((Suit)suit, cardNumber, cardValue));
        }
    }
}

void PrintAllCards()
{
    foreach (var card in cardList.OrderBy(cl => cl.Position))
    {
        TypewriterEffect($"{card.Number} of {card.Suit}");
    }
}

int GetPosition(List<int> positionsToExclude, int maxCount)
{
    var position = new Random().Next(1, maxCount);
    if (positionsToExclude.Contains(position))
    {
        return GetPosition(positionsToExclude, maxCount);
    }
    positionsToExclude.Add(position);
    return position;
}

void TypewriterEffect(string message, int speed = 10)
{
    for (int i = 0; i < message.Length; i++)
    {
        Console.Write(message[i]);
        Thread.Sleep(speed);
    }
    Console.WriteLine("");
}
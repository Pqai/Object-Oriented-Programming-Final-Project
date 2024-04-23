using GameProgram;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Drawing;

//CARD GAME IDEAS
//uno
namespace GameProgram { 

    public enum CardColour
    {
        Blue,
        Green,
        Red,
        Yellow,
        Wild
    }
    public enum CardType
    {

        Number,
        Skip,
        ChangeColour,
        PlusTwo
    }

    public int CheckUno(List<CardsBase> hand)//checking if you have 1 card left
    {

        if (hand.Count == 1) 
        { 
        Console.WriteLine("UNO!!!");
        return 1;
        }
        else
        {
            return 0;
        }
    }

    public virtual void Reset()
    {

    }

    public abstract class CardsBase
    {

        public CardType Type { get; set; }

        public CardsBase(CardType type)
        {
            Type = type;

        }
    }

    public class NumberCard : CardsBase//number and colour for card
    {
        public int Number { get; set; }
        public int Color { get; set; }

        public NumberCard(int number, int color) : base(CardType.Number)
        {
            Number = number;
            Color = color;
        }
    }

    public class Skip : CardsBase//skip card
    {

        public Skip(CardColour color) : base(CardType.Skip) 
        {
            Color = color;
        }


        public CardColour Color { get; }

        public override string ToString()
        {
            return $"Skip ({Color})";
        }
    }

    public class ChangeColour : CardsBase
    {
        public ChangeColour() : base(CardType.ChangeColour)
        {

        }

        public override string ToString()
        {
            return "Change Colour (Wild)";
        }
    }

    public class PlusTwo : CardsBase
    {
        public PlusTwo(CardColour color) : base(CardType.PlusTwo)
        {
            Color = color;
        }

        public CardColour Color { get; }

        public override string ToString()
        {
            return $"PlusTwo ({Color})";
        }
    }

    public class Deck
    {

        private List<CardsBase> cards;
        private Random rng;

        public Deck()
        {
            cards = new List<CardsBase>();
            rng = new Random();
            GenerateDeck();
            ShuffleDeck();
        }
        private void GenerateDeck()
        {
            foreach(CardColour color in Enum.GetValues(typeof(CardColour)))
            {
                if (color != CardColour.Wild)
                {
                    for(int number = 0; number < 10; number++)
                    {
                        cards.Add(new NumberCard(number, (int)color));
                        cards.Add(new NumberCard(number, (int)color));
                    }

                    for (int i = 0; i< 4; i++)
                    {
                        cards.Add(new Skip(color));
                        cards.Add(new ChangeColour());
                        cards.Add(new PlusTwo(color));
                    }
                }
            }
        }

        private void ShuffleDeck()
        {
            int n = cards.Count;
            while (n > 1)
            {

                n--;
                int k = rng.Next(n + 1);
                CardsBase value = cards[k];
                cards[k] = value;
                cards[n] = value;
            }
        }

        public CardsBase Draw()
        {

            if (cards.Count == 0)
            {
                throw new InvalidOperationException("Deck is Empty!!");//this is because i dont want the game to last long so no putting the pile back into the deck.
            }

            CardsBase drawnCard = cards[0];
            cards.RemoveAt(0);
            return drawnCard;
        }


    }

    public class Player
    {

        private List<CardsBase> hand;

        public Player()
        {
            hand = new List<CardsBase>();
        }

        public void AddToHand(CardsBase card)
        {
            hand.Add(card);
        }

        public void PrintHand()
        {
            Console.WriteLine("Your Hand: ");
            for(int i = 0; i < hand.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {hand[i].ToString()}");
            }
        }
    }

    public class HumanPlayer : Player
    {
        public HumanPlayer(string name) : base()
        {
            Console.WriteLine($"{name}, you are real.");
        }
    }

    public class ComputerPlayer : Player
    {
        public ComputerPlayer(string name) : base()
        {
            Console.WriteLine($"{name}, you are a CPU.");
        }
    }

    public class Pile
    {
        private List<CardsBase> cardsOnPile;

        public Pile()
        {
            cardsOnPile = new List<CardsBase>();
        }

        public void AddToPile(CardsBase card)
        {
            cardsOnPile.Add(card);
        }

        public bool CanPlayCard(CardsBase card)
        {
            if(card is NumberCard numberCard)
            {
                if (cardsOnPile.Count > 0 && cardsOnPile.Last() is NumberCard topNumberCards)
                {
                    return numberCard.Number == topNumberCards.Number || numberCard.Color == topNumberCards.Color;

                }
                return true;
            }
            else if(card is ChangeColour || card is Skip || card is PlusTwo)
            {
                return true;
            }
        return false;
        }
    }

    public class Game
    {
        private List<Player> players;
        private Deck deck;

        public Game(int numPlayers, bool[] isHuman)
        {
            if(numPlayers < 2 || numPlayers > 4)
            {
                throw new ArgumentException("That is an incorrect amount of players");
            }

            if (isHuman.Length != numPlayers)
            {
                throw new ArgumentException("You choice human players must be between etither 0 or 4.");
            }

            players = new List<Player>();
            deck = new Deck();

            for (int i = 0; i < numPlayers; i++)
            {
                if (isHuman[i])
                {
                    Console.Write($"Enter a name {i + 1}: ");
                    string playerName = Console.ReadLine();
                    players.Add(new HumanPlayer(playerName));
                }
                else
                {
                    players.Add(new ComputerPlayer($"Computer {i + 1}"));
                }
            }
        }

        private void DealInitialCards(Player player)
        {
            for (int i =0; i<7; i++)//deal 7 cards for each hand
            {
                player.AddToHand(deck.Draw());
            }
        }

        public void StartGame()
        {
            Console.WriteLine("Game setting up");

            foreach(var player in players)
            {
                DealInitialCards(player);
            }
        }
    }

    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("You are playing UNO");
            Console.WriteLine("\nEnter how many will be joining this game?");
            int numPlayers = int.Parse(Console.ReadLine());

            bool[] isHuman = new bool[numPlayers]; 
            for (int i =0; i < numPlayers; i++)
            {
                Console.Write($"Is player {i + 1} human? (True/False): ");
                isHuman[i] = bool.Parse(Console.ReadLine());
            }

            Game game = new Game(numPlayers, isHuman);
            game.StartGame();

        }
    }
}
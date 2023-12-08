using System.Data;

namespace Playground.aoc2023.t7.part1;

public class Task7Part1
{
    public void Main()
    {
        var fileName = "1.txt";
        // fileName = "2.txt";
        var fullFilePath = Path.Combine(Directory.GetCurrentDirectory(), "aoc2023", "t7", fileName);
        if (!File.Exists(fullFilePath))
        {
            throw new FileNotFoundException($"Can't find input file at {fullFilePath}!");
        }

        var lines = File.ReadAllLines(fullFilePath)
            .Where(x => !x.StartsWith("#") &&
                        !String.IsNullOrWhiteSpace(x)).ToArray();

        CalcPart(lines, true);// 6440 && 249483956
    }

    private void CalcPart(String[] lines, Boolean print = false)
    {
        var input = ParseInput(lines);
        var res =
            CalcHandsRank(input, print)
                .OrderByDescending(x => x.Rank);


        if (print)
        {
            foreach (var hand in res)
            {
                Console.WriteLine(hand);
            }
        }
        Console.WriteLine($"Total winnings: {res.Sum(x => x.Rank * x.Bid)}");

    }

    private List<Hand> CalcHandsRank(Input input, Boolean print = false)
    {
        var res = input.Hands.OrderByDescending(h => h, new HandComparer()).ToList();
        var maxRank = input.Hands.Count;
        var currentRank = maxRank;
        foreach (var h in res)
        {
            h.Rank = maxRank;
            maxRank--;
        }

        return res;
    }

    private Input ParseInput(String[] lines)
    {
        var input = new Input();
        foreach (var line in lines)
        {
            var inp = line.Split(" ");
            var cardsDealt = inp[0];
            var bid = Int32.Parse(inp[1]);
            input.Hands.Add(new Hand(cardsDealt, bid));
        }
        return input;
    }


}

class Input
{
    public List<Hand> Hands { get; set; } = new();
}

class Hand
{
    public Hand(String cardsDealt, Int32 bid)
    {
        CardsDealt = cardsDealt;
        Bid = bid;
        Combination = CalculateCombination();
    }
    public String CardsDealt { get; set; }
    public Int32 Bid { get; set; }
    public Combination Combination { get; set; }
    public Int32 Rank { get; set; } = 0;

    Combination CalculateCombination()
    {
        if (String.IsNullOrWhiteSpace(CardsDealt) || CardsDealt.Length != 5)
            throw new DataException($"Invalid value of {nameof(CardsDealt)}");

        var allSame = CardsDealt.All(x => x == CardsDealt[0]);
        if (allSame)
            return Combination.FiveOfAKind;

        var any4Same = CardsDealt.GroupBy(x => x).Any(g => g.Count() == 4);
        if (any4Same)
            return Combination.FourOfAKind;

        var any3Same = CardsDealt.GroupBy(x => x).Any(g => g.Count() == 3);
        if (any3Same)
        {
            var threeSame = CardsDealt.GroupBy(x => x).First(g => g.Count() == 3).Key;
            var remainingTwo = CardsDealt.Where(x => x != threeSame).ToList();
            var remainingTwoAreSame = remainingTwo[0] == remainingTwo[1];
            if (remainingTwoAreSame)
                return Combination.FullHouse;

            return Combination.ThreeOfAKind;
        }
        var any2Same = CardsDealt.GroupBy(x => x).Any(g => g.Count() == 2);

        if (any2Same)
        {
            var twoSame = CardsDealt.GroupBy(x => x).First(g => g.Count() == 2).Key;
            var remainingChars = CardsDealt.Where(x => x != twoSame).ToList();
            var twoMoreSame = remainingChars.GroupBy(x => x).Any(g => g.Count() == 2);
            if (twoMoreSame)
                return Combination.TwoPair;
            return Combination.OnePair;
        }

        return Combination.HighCard;
    }

    public override String ToString()
    {
        return $"{CardsDealt} {Bid}";
        // return $"{CardsDealt} {Bid} \t{Combination} \tr:{Rank}";
    }
}

public enum Combination
{
    HighCard,
    OnePair,
    TwoPair,
    ThreeOfAKind,
    FullHouse,
    FourOfAKind,
    FiveOfAKind
}

static class HandExtensions
{
    public static Boolean IsHigher(this Hand h1, Hand h2)
    {
        if (!h1.Combination.Equals(h2.Combination))
            return h1.Combination > h2.Combination;

        switch (h1.Combination)
        {
            case Combination.HighCard:
            case Combination.OnePair:
            case Combination.TwoPair:
            case Combination.ThreeOfAKind:
            case Combination.FullHouse:
            case Combination.FourOfAKind:
                for (int i = 0; i < h1.CardsDealt.Length; i++)
                {
                    var ch1 = h1.CardsDealt[i];
                    var ch2 = h2.CardsDealt[i];
                    if (ch1.Equals(ch2))
                        continue;
                    var isHigher = ch1.IsHigherCard(ch2);
                    return isHigher;
                }
                return false;
            case Combination.FiveOfAKind:
                return true;
            default:
                throw new ArgumentOutOfRangeException("Unknown combination.");
        }
    }

    public static char FindHighestLetter(this String cards)
    {
        var highest = cards[0];
        for (int i = 1; i < cards.Length; i++)
        {
            var card = cards[i];
            if (card.IsHigherCard(highest))
                highest = card;
        }

        return highest;
    }

    public static Boolean IsHigherCard(this Char letter1, Char letter2)
    {
        var letters = new List<Char> { 'A', 'K', 'Q', 'J', 'T', '9', '8', '7', '6', '5', '4', '3', '2' };
        var index1 = letters.IndexOf(letter1);
        var index2 = letters.IndexOf(letter2);
        if (index1 < index2)
            return true;
        return false;
    }
}

class HandComparer : IComparer<Hand>
{
    public int Compare(Hand h1, Hand h2)
    {
        if (h1 == null || h2 == null)
        {
            throw new ArgumentNullException();
        }

        if (h1.IsHigher(h2))
        {
            return 1;
        }
        else if (h2.IsHigher(h1))
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }
}
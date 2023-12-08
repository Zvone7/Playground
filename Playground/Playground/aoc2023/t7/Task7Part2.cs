using System.Data;
using Playground.aoc2023.t7.part1;

namespace Playground.aoc2023.t7.part2;

public class Task7Part2
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

        CalcPart(lines, true);// 5905 && 252137472
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

public class Hand
{
    public Hand(String cardsDealt, Int32 bid)
    {
        CardsDealt = cardsDealt;
        Bid = bid;
        CardsModified = CardsDealt;
        Combination = CalculateCombination();
    }
    public String CardsDealt { get; set; }
    public String CardsModified { get; set; }
    public Int32 Bid { get; set; }
    public Combination Combination { get; set; }
    public Int32 Rank { get; set; } = 0;

    Combination CalculateCombination()
    {
        if (String.IsNullOrWhiteSpace(CardsDealt) || CardsDealt.Length != 5)
            throw new DataException($"Invalid value of {nameof(CardsDealt)}");

        var jokersContained = CardsDealt.Count(x => x.Equals('J'));

        var combination = Combination.HighCard;

        var allSame = CardsDealt.All(x => x == CardsDealt[0]);
        if (allSame)
            combination = Combination.FiveOfAKind;
        else
        {
            var any4Same = CardsDealt.GroupBy(x => x).Any(g => g.Count() == 4);
            if (any4Same)
                combination = Combination.FourOfAKind;
            else
            {
                var any3Same = CardsDealt.GroupBy(x => x).Any(g => g.Count() == 3);
                if (any3Same)
                {
                    var threeSame = CardsDealt.GroupBy(x => x).First(g => g.Count() == 3).Key;
                    var remainingTwo = CardsDealt.Where(x => x != threeSame).ToList();
                    var remainingTwoAreSame = remainingTwo[0] == remainingTwo[1];
                    if (remainingTwoAreSame)
                        combination = Combination.FullHouse;

                    else
                    {
                        combination = Combination.ThreeOfAKind;
                    }
                }
                else
                {

                    var any2Same = CardsDealt.GroupBy(x => x).Any(g => g.Count() == 2);

                    if (any2Same)
                    {
                        var twoSame = CardsDealt.GroupBy(x => x).First(g => g.Count() == 2).Key;
                        var remainingChars = CardsDealt.Where(x => x != twoSame).ToList();
                        var twoMoreSame = remainingChars.GroupBy(x => x).Any(g => g.Count() == 2);
                        if (twoMoreSame)
                            combination = Combination.TwoPair;
                        else
                            combination = Combination.OnePair;
                    }
                }
            }
        }
        if (!jokersContained.Equals(0))
        {
            if (combination == Combination.FiveOfAKind)
            {
                CardsModified = "AAAAA";
            }
            else if (combination == Combination.FourOfAKind)
            {
                combination = Combination.FiveOfAKind;
                var repeatingChar = CardsDealt.GroupBy(x => x).First(g => g.Count() == 4).Key;
                var uniqueChar = CardsDealt.First(x => x != repeatingChar);

                CardsModified = CardsDealt.Replace(uniqueChar, repeatingChar);
                if (CardsModified.Equals("JJJJJ"))
                    CardsModified = "AAAAA";
            }
            else if (combination == Combination.FullHouse)
            {
                if (jokersContained.Equals(2))
                {
                    // QQQJJ
                    var repeatingChar = CardsDealt.GroupBy(x => x).First(g => g.Count() == 3).Key;
                    combination = Combination.FiveOfAKind;
                    CardsModified = CardsDealt.Replace('J', repeatingChar);
                    // QQQQQ
                }
                else if (jokersContained.Equals(3))
                {
                    // JJJQQ
                    var repeatingChar = CardsDealt.GroupBy(x => x).First(g => g.Count() == 2).Key;
                    combination = Combination.FiveOfAKind;
                    CardsModified = CardsDealt.Replace('J', repeatingChar);
                }
                else
                {
                    throw new DataException("Not supposed to be here - not 3 or 2 jokers on full house??");
                }
            }
            else if (combination == Combination.ThreeOfAKind)
            {
                combination = Combination.FourOfAKind;
                var repeatingChar = CardsDealt.GroupBy(x => x).First(g => g.Count() == 3).Key;
                if (repeatingChar == 'J')
                {
                    // JJJ45
                    var uniqueChars = CardsDealt.Where(x => x != 'J').ToArray();
                    var higherChar = uniqueChars[1];
                    if (uniqueChars[0].IsHigherCard(higherChar))
                        higherChar = uniqueChars[0];
                    CardsModified = CardsDealt.Replace('J', higherChar);
                    // 55545
                }
                else
                {
                    if (jokersContained.Equals(1))
                    {
                        // 111J5
                        combination = Combination.FourOfAKind;
                        CardsModified = CardsDealt.Replace('J', repeatingChar);
                        // 11115
                    }
                    else
                    {
                        // 111JJ
                        combination = Combination.FiveOfAKind;
                        var uniqueChar = CardsDealt.First(x => x != repeatingChar && x != 'J');
                        CardsModified = CardsDealt.Replace('J', uniqueChar);
                        // 11111
                    }
                }
            }
            else if (combination == Combination.TwoPair)
            {
                if (jokersContained.Equals(1))
                {
                    // 11J22
                    var nonJChars = CardsDealt
                        .Where(x => x != 'J')
                        .GroupBy(y => y)
                        .Select(z => new { Char = z.Key, Count = z.Count() })
                        .ToList();
                    var ch1 = nonJChars.First().Char;
                    var ch2 = nonJChars.Last().Char;
                    var higherChar = ch1;
                    if (ch2.IsHigherCard(ch1))
                        higherChar = ch2;

                    combination = Combination.FullHouse;
                    CardsModified = CardsDealt.Replace('J', higherChar);
                    // 11222
                }
                else if (jokersContained.Equals(2))
                {
                    // 112JJ
                    var repeatingChar = CardsDealt.GroupBy(x => x).First(g => g.Count() == 2 && g.Key != 'J').Key;
                    combination = Combination.FourOfAKind;
                    CardsModified = CardsDealt.Replace('J', repeatingChar);
                    // 11211
                }
                else
                {
                    throw new DataException("Not supposed to be here - not 1 or 2 jokers on two pairs??");
                }
            }
            else if (combination == Combination.OnePair)
            {
                if (jokersContained.Equals(1))
                {
                    combination = Combination.ThreeOfAKind;
                    var repeatingChar = CardsDealt.GroupBy(x => x).First(g => g.Count() == 2).Key;
                    var uniqueChar = CardsDealt.First(x => x != repeatingChar && x != 'J');
                    CardsModified = CardsDealt.Replace('J', repeatingChar);
                }
                if (jokersContained.Equals(2))
                {
                    // JJA12
                    combination = Combination.ThreeOfAKind;
                    var repeatingChar = CardsDealt.GroupBy(x => x).First(g => g.Count() == 2).Key;
                    var nonJChars = CardsDealt.Where(x => x != 'J');
                    var highestLetter = new String(nonJChars.ToArray()).FindHighestLetter();
                    CardsModified = CardsDealt.Replace('J', highestLetter);
                    // AAA12
                }

            }
            else if (combination == Combination.HighCard)
            {
                if (jokersContained.Equals(1))
                {
                    // A123J
                    var nonJChars = CardsDealt.Where(x => x != 'J');
                    var s = new String(nonJChars.ToArray());
                    var highestLetter = s.FindHighestLetter();
                    CardsModified = CardsDealt.Replace('J', highestLetter);
                    combination = Combination.OnePair;
                    // A123A
                }
                else
                {
                    throw new DataException("Should not be here - multiple J on highest card??");
                }

            }

        }

        return combination;

    }

    public override String ToString()
    {
        // return $"{CardsDealt} {Bid}";
        return $"{CardsDealt} {CardsModified} {Bid} \t{Combination} \tr:{Rank}";
    }
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
    public static Boolean IsHigher2(this Hand h1, Hand h2)
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
        var letters = new List<Char> { 'A', 'K', 'Q', 'T', '9', '8', '7', '6', '5', '4', '3', '2', '1', 'J' };
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
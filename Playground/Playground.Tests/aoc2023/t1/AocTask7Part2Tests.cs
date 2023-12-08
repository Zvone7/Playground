using Playground.aoc2023.t1;
using Playground.aoc2023.t6.part1;
using Playground.aoc2023.t6.part2;

namespace Playground.Tests;

public class AocTask7Part2Tests
{
    [Theory]
    [InlineData("AAAAA", "AAAAA", Combination.FiveOfAKind)]
    [InlineData("AAAA1", "AAAA1", Combination.FourOfAKind)]
    [InlineData("AAA11", "AAA11", Combination.FullHouse)]
    [InlineData("11AAA", "11AAA", Combination.FullHouse)]
    [InlineData("AAA12", "AAA12", Combination.ThreeOfAKind)]
    [InlineData("AA1KK", "AA1KK", Combination.TwoPair)]
    [InlineData("AA123", "AA123", Combination.OnePair)]
    [InlineData("AK123", "AK123", Combination.HighCard)]
    //
    [InlineData("JJJJJ", "AAAAA", Combination.FiveOfAKind)]
    [InlineData("JJJJA", "AAAAA", Combination.FiveOfAKind)]
    [InlineData("AAAJJ", "AAAAA", Combination.FiveOfAKind)]
    [InlineData("AAAJ1", "AAAA1", Combination.FourOfAKind)]
    [InlineData("AAJKK", "AAAKK", Combination.FullHouse)]
    [InlineData("AA1JJ", "AA1AA", Combination.FourOfAKind)]
    [InlineData("AAJ12", "AAA12", Combination.ThreeOfAKind)]
    [InlineData("JJA12", "AAA12", Combination.ThreeOfAKind)]
    [InlineData("JA123", "AA123", Combination.OnePair)]
    public void Test(String testCardsDealt, String expectedCardsModified, Combination expectedCombination)
    {
        var hand = new Hand(testCardsDealt, 1);

        Assert.Equal(expectedCombination, hand.Combination);
        Assert.Equal(expectedCardsModified, hand.CardsModified);
    }
}
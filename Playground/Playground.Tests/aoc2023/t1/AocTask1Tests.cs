using Playground.aoc2023.t1;

namespace Playground.Tests;

public class AocTask1Tests
{
    [Theory]
    [InlineData("11", 11)]
    [InlineData("oneone", 11)]
    [InlineData("1", 11)]
    [InlineData("one", 11)]
    [InlineData("xzyone2", 12)]
    [InlineData("onetwo", 12)]
    [InlineData("twoone", 21)]
    [InlineData("twone", 21)]
    [InlineData("oneight", 18)]
    [InlineData("oneeight", 18)]
    [InlineData("threeeight", 38)]
    [InlineData("threeight", 38)]
    [InlineData("fiveeight", 58)]
    [InlineData("fiveight", 58)]
    [InlineData("eightthree", 83)]
    [InlineData("eighthree", 83)]
    [InlineData("eighttwo", 82)]
    [InlineData("eightwo", 82)]
    [InlineData("nineeight", 98)]
    [InlineData("nineight", 98)]
    public void Test(String testLine, Int32 expected)
    {
        var task = new Task1();
        var actual = task.ProcessLinePart2(testLine);

        Assert.True(actual.Equals(expected));
    }
}
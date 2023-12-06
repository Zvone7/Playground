using System.Collections.Concurrent;
using System.Numerics;

namespace Playground.aoc2023.t6;

public class Task6Part2
{
    public void Main()
    {
        var fileName = "1.txt";
        fileName = "2.txt";
        var fullFilePath = Path.Combine(Directory.GetCurrentDirectory(), "aoc2023", "t6", fileName);
        if (!File.Exists(fullFilePath))
        {
            throw new FileNotFoundException($"Can't find input file at {fullFilePath}!");
        }

        var lines = File.ReadAllLines(fullFilePath);

        CalcPart(lines, true);// 71503 && 29891250
    }

    private void CalcPart(String[] lines, Boolean print = false)
    {
        var input = ParseInput(lines);
        var res = CalculateAllDistancesPerRace(input);
        // var res = CalculateAllDistancesPerRaceParallel(input);
        Console.WriteLine($"Final result: {res}");
    }

    private Int64 CalculateAllDistancesPerRace(Input input)
    {
        Int64 result = 0;

        for (int i = 0; i < input.Time; i++)
        {
            var r = CalculateDistanceCrossed(i, input.Time);
            if (r > input.Distance)
                result++;
        }

        return result;
    }

    Int64 CalculateAllDistancesPerRaceParallel(Input input)
    {
        Int64 result = 0;

        Parallel.For(0, input.Time, i =>
        {
            var r = CalculateDistanceCrossed(i, input.Time);
            if (r > input.Distance)
                Interlocked.Increment(ref result);
        });

        return result;
    }

    private Int64 CalculateDistanceCrossed(Int64 pushTime, Int64 raceTime)
    {
        var speed = pushTime;
        var x = (raceTime - pushTime) * speed;
        return x;
    }
    void PrintHelp(List<(Int32 pushTime, Int32 distanceCrossed)> list, Boolean print = false)
    {
        if (print)
        {
            foreach (var tuple in list)
            {
                Console.WriteLine($"hold:{tuple.pushTime}\t\tdist:{tuple.distanceCrossed}");
            }
        }
    }

    private Input ParseInput(String[] lines)
    {
        var input = new Input();
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (line.StartsWith("#")) continue;
            if (line.StartsWith("Time:"))
            {
                var timeString = line.Split("Time:")[1].Replace(" ", String.Empty);
                var r = Int32.TryParse(timeString, out var n);
                if (r)
                    input.Time = n;

            }
            else if (line.StartsWith("Distance:"))
            {
                var distString = line.Split("Distance:")[1].Replace(" ", String.Empty);
                var r = Int64.TryParse(distString, out var n);
                if (r)
                    input.Distance = n;
            }
        }

        return input;
    }

    class Input
    {
        public Int64 Time { get; set; }
        public Int64 Distance { get; set; }

    }
}
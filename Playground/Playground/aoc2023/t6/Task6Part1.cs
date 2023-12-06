namespace Playground.aoc2023.t6;

public class Task6Part1
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

        CalcPart(lines, false);// 288 && 2612736
    }

    private void CalcPart(String[] lines, Boolean print = false)
    {
        var input = ParseInput(lines);

        var res = CalculateAllRaces(input);
        PrintHelp(res, print);
        var res2 = CalculateResult(input, res);
        Console.WriteLine($"Final result: {res2}");
    }

    private Int32 CalculateResult(
        Input input,
        List<List<(Int32 pushTime, Int32 distanceCrossed)>> all,
        Boolean print = false)
    {
        var res = 1;
        for (var i = 0; i < all.Count; i++)
        {
            var race = all[i];
            var ri = input.Distances[i];
            var winningComboCount = race
                .OrderBy(x => x.distanceCrossed)
                .Count(x => x.distanceCrossed > ri);
            if (print)
                Console.WriteLine($"{winningComboCount} attempts good enough to beat the record of d:{input.Distances[i]}.");
            res = res * winningComboCount;
        }

        return res;
    }

    private List<List<(Int32 pushTime, Int32 distanceCrossed)>> CalculateAllRaces(Input input)
    {
        var result = new List<List<(Int32 pushTime, Int32 distanceCrossed)>>();
        var times = input.Times.ToArray();
        var distances = input.Distances.ToArray();
        for (var i = 0; i < distances.Length; i++)
        {
            result.Add(CalculateAllDistancesPerRace(times[i], distances[i]));
        }

        return result;
    }

    private List<(Int32 pushTime, Int32 distanceCrossed)> CalculateAllDistancesPerRace(Int32 raceDistance, Int32 raceTime)
    {
        var possibleButtonPushTimes = Enumerable.Range(0, raceDistance + 1);
        var results = possibleButtonPushTimes
            .Select(po => (po, CalculateDistanceCrossed(po, raceDistance)))
            .ToList();
        return results;
    }

    private Int32 CalculateDistanceCrossed(Int32 pushTime, Int32 raceTime)
    {
        var speed = pushTime;
        return (raceTime - pushTime) * pushTime;
    }
    void PrintHelp(List<List<(Int32 pushTime, Int32 distanceCrossed)>> list, Boolean print = false)
    {
        if (print)
        {
            for (var ri = 0; ri < list.Count; ri++)
            {
                Console.WriteLine("---");
                foreach (var tuple in list[ri])
                {
                    Console.WriteLine($"[{ri}]\thold:{tuple.pushTime}\tdist:{tuple.distanceCrossed}");
                }
            }
        }
    }

    private Input ParseInput(String[] lines)
    {
        var input = new Input();
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (line.StartsWith("Time:"))
            {
                var timeStrings = line.Split("Time:")[1].Split(" ");
                foreach (var ts in timeStrings)
                {
                    var r = Int32.TryParse(ts, out var n);
                    if (r)
                        input.Times.Add(n);
                }
            }
            else if (line.StartsWith("Distance:"))
            {
                var distStrings = line.Split("Distance:")[1].Split(" ");
                foreach (var ts in distStrings)
                {
                    var r = Int32.TryParse(ts, out var n);
                    if (r)
                        input.Distances.Add(n);
                }
            }
        }

        return input;
    }

    class Input
    {
        public List<Int32> Times { get; set; } = new();
        public List<Int32> Distances { get; set; } = new();

    }
}
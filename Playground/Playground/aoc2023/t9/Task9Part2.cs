#nullable enable
using System.Data;
using System.Text;

namespace Playground.aoc2023.t9.part2;

public class Task9Part2
{
    public void Main()
    {
        var fileName = "1.txt";
        fileName = "2.txt";
        var fullFilePath = Path.Combine(Directory.GetCurrentDirectory(), "aoc2023", "t9", fileName);
        if (!File.Exists(fullFilePath))
        {
            throw new FileNotFoundException($"Can't find input file at {fullFilePath}!");
        }

        var lines = File.ReadAllLines(fullFilePath)
            .Where(x => !x.StartsWith("#") &&
                        !String.IsNullOrWhiteSpace(x)).ToArray();

        CalcPart(lines, false);// 2 && 1097
    }

    private void CalcPart(String[] lines, Boolean print = false)
    {
        var input = ParseInput(lines);
        PrintHelp(input, print);
        foreach (var sequence in input.Sequences)
        {
            CalculateAllSubSequences(sequence);
        }

        PrintHelp(input, print);

        foreach (var sequence in input.Sequences)
        {
            CalculateAllLastAndFirstNumbers(sequence);
        }

        PrintHelp(input, print);

        Console.WriteLine($"Total: {input.Sequences.Sum(x => x.Main.First())}");

    }

    private void PrintHelp(Input input, Boolean print = false)
    {
        foreach (var sequence in input.Sequences)
        {
            if (print) PrintSingleSequence(sequence);
        }
        if (print) Console.WriteLine($"-------------");
    }

    private void PrintSingleSequence(Sequence seq)
    {
        Console.WriteLine(String.Join(" ", seq.Main));
        for (var i = 0; i < seq.Subsequences.Count; i++)
        {
            var tabbing = new StringBuilder();
            for (var j = 0; j <= i; j++)
            {
                tabbing.Append(' ');
            }
            var subsequence = seq.Subsequences[i];
            tabbing.Append(String.Join(" ", subsequence));
            Console.WriteLine(tabbing.ToString());
        }
    }

    private void CalculateAllLastAndFirstNumbers(Sequence seq)
    {
        for (var i = seq.Subsequences.Count - 2; i >= 0; i--)
        {
            var ss = seq.Subsequences[i];
            var nextSs = seq.Subsequences[i + 1];
            var lastNumberToAdd = nextSs.Last() + ss.Last();
            var firstNumberToAdd = ss.Skip(1).First() - nextSs.First();
            ss.Add(lastNumberToAdd);
            ss[0] = firstNumberToAdd;
        }
        seq.Main.Add(seq.Subsequences.First().Last() + seq.Main.Last());
        seq.Main[0] = seq.Main[1] - seq.Subsequences.First().First();
    }

    private void CalculateAllSubSequences(Sequence seq)
    {
        Boolean shouldRunLoop = false;
        var containsSubsequences = seq.Subsequences.Any();
        var lastSubsequenceContainsNumbers = false;
        var lastSubSequence = seq.Subsequences.LastOrDefault();
        if (lastSubSequence != null)
            lastSubsequenceContainsNumbers = lastSubSequence.Any(x => x != 0);

        shouldRunLoop = !containsSubsequences || lastSubsequenceContainsNumbers;

        while (shouldRunLoop)
        {
            var inputToUse = seq.Main;
            if (seq.Subsequences.Any())
                inputToUse = seq.Subsequences.Last();
            var subSequence = CalcSubsequence(inputToUse.Skip(1).ToList());

            seq.Subsequences.Add(subSequence);

            lastSubSequence = seq.Subsequences.LastOrDefault();
            if (lastSubSequence != null)
                lastSubsequenceContainsNumbers = lastSubSequence.Any(x => x != 0);

            shouldRunLoop = lastSubsequenceContainsNumbers;
        }
    }

    private List<Int32> CalcSubsequence(List<Int32> list)
    {
        var res = new List<Int32>();
        res.Add(0);
        for (var i = 1; i < list.Count; i++)
        {
            var n2 = list[i];
            var n = list[i - 1];

            res.Add(n2 - n);
        }
        return res;
    }


    public Input ParseInput(String[] lines)
    {
        var input = new Input();
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var numberStrings = line.Split(" ");
            var numbers = new List<Int32>();
            numbers.Add(0);
            foreach (var ns in numberStrings)
            {
                var r = Int32.TryParse(ns, out var n);
                if (r)
                    numbers.Add(n);
            }
            input.Sequences.Add(new Sequence()
                {
                    Main = numbers,
                    Subsequences = new List<List<Int32>>()
                }
            );
        }


        return input;
    }


}

public class Input
{
    public List<Sequence> Sequences { get; set; } = new();
}

public class Sequence
{
    public List<int> Main { get; set; }
    public List<List<int>> Subsequences { get; set; }
}
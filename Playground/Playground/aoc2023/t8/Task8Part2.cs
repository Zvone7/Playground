#nullable enable
using System.Data;
using Playground.aoc2023.t8.part1;

namespace Playground.aoc2023.t8.part2;

public class Task8Part2
{
    public void Main()
    {
        var fileName = "4.txt";
        fileName = "5.txt";
        var fullFilePath = Path.Combine(Directory.GetCurrentDirectory(), "aoc2023", "t8", fileName);
        if (!File.Exists(fullFilePath))
        {
            throw new FileNotFoundException($"Can't find input file at {fullFilePath}!");
        }

        var lines = File.ReadAllLines(fullFilePath)
            .Where(x => !x.StartsWith("#") 
                        // && !String.IsNullOrWhiteSpace(x)
                        ).ToArray();

        CalcPart(lines, false);// (4) 6 && (5) 12324145107121
    }

    private void CalcPart(String[] lines, Boolean print = false)
    {
        var t = new Task8Part1();
        var input = t.ParseInput(lines);

        var startNodesOriginal = input.Nodes.Where(x => x.Name.EndsWith("A")).ToList();
        var startNodes = startNodesOriginal.Select(x => x.DeepCopy()).ToList();

        var setsDone = 0;

        while (startNodes.Count - startNodes.Where(x => x.Name.EndsWith("Z")).ToList().Count > 0)
        {
            foreach (var instruction in input.Instructions)
            {
                if (instruction == 'L')
                {
                    for (var i = 0; i < startNodes.Count; i++)
                    {
                        var n = startNodes[i];
                        n = n.Left;
                        startNodes[i] = n;
                        if (print) Console.WriteLine($"[{i}]({setsDone})Moved to {n.Name}");
                    }
                }
                else if (instruction == 'R')
                {
                    for (var i = 0; i < startNodes.Count; i++)
                    {
                        var n = startNodes[i];
                        n = n.Right;
                        startNodes[i] = n;
                        if (print) Console.WriteLine($"[{i}]({setsDone})Moved to {n.Name}");
                    }
                }
                else
                    throw new Exception("unknown instruction");

            }
            setsDone++;
            if (setsDone % 100_000 == 0)
                Console.WriteLine($"On set {setsDone}");
        }

        Console.WriteLine($"Total instructions: {setsDone * input.Instructions.Length}");

    }

}
using System.ComponentModel;
using System.Data;
using System.Diagnostics;

namespace Playground.aoc2023.t3;

public class Task6
{
    public void Main()
    {
        var fileName = "1.txt";
        fileName = "2.txt";
        var fullFilePath = Path.Combine(Directory.GetCurrentDirectory(), "aoc2023", "t5", fileName);
        if (!File.Exists(fullFilePath))
        {
            throw new FileNotFoundException($"Can't find input file at {fullFilePath}!");
        }

        var lines = File.ReadAllLines(fullFilePath);

        // CalcPart1(lines, true);// 35 && 535088217
        CalcPart2(lines, true);// 46
    }


    private void CalcPart2(String[] lines, Boolean print = false)
    {
        var input = ExtractLineData(lines);
        // PrintHelp(input, print);
       
    }

    private void CalcPart1(String[] lines, Boolean print = false)
    {
        var input = ExtractLineData(lines);
        // PrintHelp(input, print);
        
    }

    void PrintHelp(Input input, Boolean print = false)
    {
        if (print)
        {
           
        }
    }

    void PrintHelp2(Boolean print = false)
    {
        // var sorted = seedInfos.OrderByDescending(x => x.LocIndex).ToList();
        // if (print)
        // {
        //     foreach (var si in sorted)
        //     {
        //         Console.WriteLine(si);
        //     }
        // }

        Console.WriteLine($"Lowest: ");
    }

    private Input ExtractLineData(String[] lines)
    {
        var input = new Input();
      

        var linesTemp = new List<String>();
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            
        }

        return input;
    }

    class Input
    {
        
    }
}
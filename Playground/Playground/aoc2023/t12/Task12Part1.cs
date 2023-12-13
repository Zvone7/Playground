#nullable enable
using System.Data;
using System.Text;
using Microsoft.VisualBasic;

namespace Playground.aoc2023.t12.part1;

public class Task12Part1
{
    public void Main()
    {
        var fileName = "1.txt";//  21
        fileName = "2.txt";//  
        // fileName = "3.txt";//  
        // fileName = "4.txt";//  
        // fileName = "5.txt";//  
        var fullFilePath = Path.Combine(Directory.GetCurrentDirectory(), "aoc2023", "t12", fileName);
        if (!File.Exists(fullFilePath))
        {
            throw new FileNotFoundException($"Can't find input file at {fullFilePath}!");
        }

        var lines = File.ReadAllLines(fullFilePath)
            .Where(
                x => !x.StartsWith("--") &&
                     !string.IsNullOrWhiteSpace(x)).ToArray();

        CalcPart(lines, false);
    }

    private void CalcPart(String[] lines, Boolean print = false)
    {
        var input = ParseInput(lines);

        var input2 = UnfoldInput(input, 5);

        var inputToUse = input2;

        var totalValidFormations = 0;
        for (var i = 0; i < inputToUse.Setups.Count; i++)
        {
            var setup = inputToUse.Setups[i];
            var setupValidFormations = FindPossibleFormations(setup);
            totalValidFormations += setupValidFormations;
            Console.WriteLine($"[{i + 1}] {setupValidFormations}");
        }


        Console.WriteLine($"Total: {totalValidFormations}");

    }

    Int32 FindPossibleFormations(Setup setup, Boolean print = false)
    {
        var validFormations = 0;

        var formationOptions = StringGenerator.GenerateOptions(setup.Formation);
        // formationOptions = new List<String>() { ".###.##.#.##" };

        foreach (var formationOption in formationOptions)
        {
            if (IsValidFormation(setup, formationOption, print))
            {
                // Console.WriteLine(formationOption);
                validFormations++;
                if (validFormations % 1000 == 0)
                    Console.WriteLine(validFormations);
            }
        }

        return validFormations;
    }

    Boolean IsValidFormation(Setup setup, String formation, Boolean print = false)
    {
        var parts = formation
            .Split(".")
            .Where(x => !String.IsNullOrWhiteSpace(x))
            .ToArray();
        var partsMatched = 0;
        if (parts.Length != setup.Groups.Count)
            return false;

        for (var i = 0; i < setup.Groups.Count; i++)
        {
            var g = setup.Groups[i];
            for (int j = i; j < parts.Length; j++)
            {
                var p = parts[j];
                if (p.Length == 0)
                    continue;
                if (p.Length != g)
                    return false;

                // check all characters in p are the same
                if (p.ToCharArray().Distinct().Count() > 1)
                    continue;

                partsMatched++;
                break;
            }
        }
        if (partsMatched.Equals(setup.Groups.Count))
        {
            if (print) Console.WriteLine($"possible formation: {formation}");
            return true;
        }
        return false;
    }

    Input UnfoldInput(Input input, Int32 foldAmount = 5)
    {
        var unfoldedInput = new Input();
        foreach (var setup in input.Setups)
        {
            // expand the string part times by itself, joined by ?
            var expandedFormation = String.Join("?", Enumerable.Repeat(setup.Formation, foldAmount));
            // do the same for an array of Ints
            var expandedGroups = Enumerable.Repeat(setup.Groups, foldAmount).SelectMany(x => x).ToList();

            unfoldedInput.Setups.Add(
                new Setup
                {
                    Formation = expandedFormation,
                    Groups = expandedGroups
                }
            );
        }

        return unfoldedInput;
    }


    Input ParseInput(String[] lines)
    {
        var input = new Input();
        var galaxiesFound = 1;
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var formation = line.Split(" ")[0];
            var groups = line.Split(" ")[1]
                .Split(",")
                .Select(gs => Int32.Parse(gs)).ToList();
            input.Setups.Add(new Setup()
            {
                Formation = formation,
                Groups = groups
            });

        }
        Console.WriteLine($"data parsed");
        return input;
    }

}

public class Input
{
    public List<Setup> Setups { get; set; } = new();
}

public class Setup
{
    public String Formation { get; set; }
    public List<Int32> Groups { get; set; }
}

public class StringGenerator
{
    public static IEnumerable<string> GenerateOptions(string input)
    {
        List<char> options = new List<char> { '#', '.' };
        return GenerateOptions(input, 0, options);
    }

    private static IEnumerable<string> GenerateOptions(string input, int index, List<char> options)
    {
        if (index >= input.Length)
        {
            yield return input;
        }
        else if (input[index] == '?')
        {
            foreach (char option in options)
            {
                string newInput = input.Substring(0, index) + option + input.Substring(index + 1);
                foreach (var result in GenerateOptions(newInput, index + 1, options))
                {
                    yield return result;
                }
            }
        }
        else
        {
            foreach (var result in GenerateOptions(input, index + 1, options))
            {
                yield return result;
            }
        }
    }
}
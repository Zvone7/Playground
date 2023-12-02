namespace Playground.aoc2023.t1;

public class Task1_debug
{
    public void Main()
    {
        // var fileName = "test.txt";
        // var fileName = "part2_input_short.txt";
        var fileName = "part2_input_full.txt";
        var fullFilePath = $"{Directory.GetCurrentDirectory()}\\aoc2023\\t1\\{fileName}";
        if (!File.Exists(fullFilePath))
        {
            throw new FileNotFoundException("Can't find input file!");
        }

        var lines = File.ReadAllLines(fullFilePath);

        // CalcPart1(lines);
        CalcPart2(lines, true);
    }

    void CalcPart1(string[] lines)
    {
        var sum = 0;

        foreach (var line in lines)
        {
            var digits = (from ch in line where char.IsDigit(ch) select int.Parse(ch.ToString())).ToList();

            switch (digits.Count)
            {
                case 1:
                {
                    var result = digits.First() * 10 + digits.First();
                    // Console.WriteLine(result);
                    sum += result;
                    break;
                }
                case > 1:
                {
                    var result = digits.First() * 10 + digits.Last();
                    // Console.WriteLine(result);
                    sum += result;
                    break;
                }
            }
        }

        Console.WriteLine(sum);
    }

    void CalcPart2(string[] lines, Boolean printEachLineResult)
    {
        var sum = 0;


        for (var i = 0; i < lines.Length; i++)
        {
            var res = ProcessLinePart2(lines[i].ToLower());
            sum += res.Item1;
            if (printEachLineResult)
            {
                Console.WriteLine($"[{i + 1}] {lines[i]}");
                Console.WriteLine($"[{i + 1}] {res.Item2}");
                Console.WriteLine($"[{i + 1}] result: {res.Item1}");
                Console.WriteLine($"---");
            }
        }

        Console.WriteLine(sum);
        // oneight read as 1
        // 54780 - too high

        // oneight read as 18
        // 54754 - too low
    }

    public (Int32, String) ProcessLinePart2(String line)
    {
        var l = line.ToLower();
        var res = ExtractAllDigits(l);
        var digits = res.Item1;
        if (digits.Contains(0))
            throw new InvalidDataException("Zero found");

        switch (digits.Count)
        {
            case 0:
                return (0, res.Item2);
            case 1:
            {
                var result = digits.First() * 10 + digits.First();
                return (result, res.Item2);
            }
            default:
            {
                var result = digits.First() * 10 + digits.Last();
                return (result, res.Item2);
            }
        }
    }

    (List<Int32>, String) ExtractAllDigits(string line)
    {
        var digitsToCheck = new List<int>();
        for (var i = 0; i < 10; i++)
        {
            digitsToCheck.Add(i);
        }

        var debugLine = new List<char>();
        var digits = new List<int>();
        for (int i = 0; i < line.Length; i++)
        {
            var letterForCheck = line[i];
            if (char.IsDigit(letterForCheck))
            {
                digits.Add(int.Parse(letterForCheck.ToString()));
                debugLine.Add(letterForCheck);
            }
            else
            {
                var foundNumber = false;
                foreach (var numb in digitsToCheck)
                {
                    var res = CheckIfNumberOnIndex(line, i, numb);
                    var shuffleIndex = res.Item1;
                    if (shuffleIndex > 0)
                    {
                        // for oneeight
                        // i += shuffleIndex;

                        // for oneight
                        i += shuffleIndex - 1;
                        digits.Add(numb);
                        for (var j = 0; j < res.Item2.Length - 1; j++)
                        {
                            var element = res.Item2[j];
                            debugLine.Add(element);
                        }

                        foundNumber = true;
                        break;
                    }
                }

                if (!foundNumber)
                {
                    debugLine.Add('_');
                }
            }
        }

        var debugLineString = new String(debugLine.ToArray());
        return (digits, debugLineString);
    }

    (Int32, String) CheckIfNumberOnIndex(string line, int index, int number)
    {
        var numberInLetters = number switch
        {
            0 => "zero",
            1 => "one",
            2 => "two",
            3 => "three",
            4 => "four",
            5 => "five",
            6 => "six",
            7 => "seven",
            8 => "eight",
            9 => "nine",
            _ => ""
        };

        if (line[index] == numberInLetters[0])
        {
            if (line.Length - index >= numberInLetters.Length - 1)
            {
                Boolean matching = true;
                var increment = 1;
                var relativeIndex = index;
                while (matching)
                {
                    var letterInLine = line[relativeIndex + increment];
                    var letterInNumber = numberInLetters[increment];
                    if (letterInLine != letterInNumber)
                        matching = false;
                    increment++;
                    if (increment == numberInLetters.Length - 1)
                        break;
                }

                if (matching)
                {
                    return (increment, number.ToString() + new String('_', numberInLetters.Length-1));
                }
            }
        }

        return (0, "");
    }
}
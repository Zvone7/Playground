using System.ComponentModel;

namespace Playground.aoc2023.t3;

public class Task3
{
    public void Main()
    {
        var fileName = "1.txt";
        fileName = "2.txt";
        var fullFilePath = Path.Combine(Directory.GetCurrentDirectory(), "aoc2023", "t3", fileName);
        if (!File.Exists(fullFilePath))
        {
            throw new FileNotFoundException($"Can't find input file at {fullFilePath}!");
        }

        var lines = File.ReadAllLines(fullFilePath);

        // CalcPart1(lines, false);
        CalcPart2(lines, false);
    }

    void CalcPart2(String[] lines, Boolean print = false)
    {
        var lineDatas = ExtractLineData(lines);

        Console.WriteLine($"scanned {lineDatas.Count} line data");

        var sum = 0;
        var solutions = new List<(Int32, Int32, Int32, Int32)>();
        foreach (var lineData in lineDatas)
        {
            if (lineData.IsTopLine)
            {
                sum += CheckCurrentLinePt2(
                    lineData,
                    null,
                    lineDatas.First(x => x.LineIndex == lineData.LineIndex + 1)
                    , solutions, print);
            }
            else if (lineData.IsBottomLine)
            {
                sum += CheckCurrentLinePt2(lineData,
                    lineDatas.First(x => x.LineIndex == lineData.LineIndex - 1),
                    null,
                    solutions, print);
            }
            else
            {
                sum += CheckCurrentLinePt2(
                    lineData,
                    lineDatas.First(x => x.LineIndex == lineData.LineIndex - 1),
                    lineDatas.First(x => x.LineIndex == lineData.LineIndex + 1),
                    solutions, print);
            }
        }
        Console.WriteLine($"Sum of parts: {sum}");
        if (print)
        {
            var sorted = solutions
                    .OrderBy(x => x.Item1)
                    .ThenBy(y => y.Item3)
                ;
            foreach (var t in sorted)
            {
                Console.WriteLine($"[{t.Item1}]{t.Item2} * [{t.Item3}]{t.Item4}");
            }
        }

    }

    private Int32 CheckCurrentLinePt2(
        LineData lineData,
        LineData? lineAbove,
        LineData? lineBelow,
        List<(Int32, Int32, Int32, Int32)> solutions, Boolean print = false)
    {
        var sum = 0;
        if (!lineData.SymbolIndexes.Any())
            return sum;
        foreach (var si in lineData.SymbolIndexes)
        {
            var result = 1;
            // above, below, left, right
            Int32[] states = new[] { 0, 0, 0, 0 };
            List<(Int32, Int32)> possibleTuples = new List<(Int32, Int32)>();

            if (lineAbove != null)
            {
                var numbersAbove = lineAbove.NumberInfos.Where(x =>
                        x.ListOfIndexesNumberIsCountedOn.Contains(si) 
                        ||
                        x.ListOfIndexesNumberIsCountedOn.Contains(si - 1) ||
                        x.ListOfIndexesNumberIsCountedOn.Contains(si + 1)
                        )
                    .ToList();
                if (numbersAbove.Any())
                {
                    if (numbersAbove.Count == 1)
                    {
                        var numbAbove = numbersAbove.FirstOrDefault();
                        if (numbAbove != null)
                        {
                            result *= numbAbove.Number;
                            possibleTuples.Add((lineAbove.LineIndex, numbAbove.Number));
                            states[0] = 1;
                        }
                    }
                    else if (numbersAbove.Count == 2)
                    {
                        var numbAbove1 = numbersAbove.First();
                        var numbAbove2 = numbersAbove.Last();
                        result *= numbAbove1.Number * numbAbove2.Number;
                        possibleTuples.Add((lineAbove.LineIndex, numbAbove1.Number));
                        possibleTuples.Add((lineAbove.LineIndex, numbAbove2.Number));
                        states[0] = 2;
                    }
                    else
                    {
                        states[0] = numbersAbove.Count;
                    }
                }
            }

            if (lineBelow != null)
            {
                var numbersBelow = lineBelow.NumberInfos.Where(x =>
                        x.ListOfIndexesNumberIsCountedOn.Contains(si) 
                        ||
                        x.ListOfIndexesNumberIsCountedOn.Contains(si - 1) ||
                        x.ListOfIndexesNumberIsCountedOn.Contains(si + 1)
                        )
                    .ToList();
                if (numbersBelow.Any())
                {
                    if (numbersBelow.Count == 1)
                    {
                        var numbBelow = numbersBelow.FirstOrDefault();
                        if (numbBelow != null)
                        {
                            result *= numbBelow.Number;
                            possibleTuples.Add((lineBelow.LineIndex, numbBelow.Number));
                            states[1] = 1;
                        }
                    }
                    else if (numbersBelow.Count == 2)
                    {
                        var numbBelow1 = numbersBelow.First();
                        var numbBelow2 = numbersBelow.Last();
                        result *= numbBelow1.Number * numbBelow2.Number;
                        possibleTuples.Add((lineBelow.LineIndex, numbBelow1.Number));
                        possibleTuples.Add((lineBelow.LineIndex, numbBelow2.Number));
                        states[1] = 2;
                    }
                    else
                    {
                        states[1] = numbersBelow.Count;
                    }
                }
            }

            var numbLeft = lineData.NumberInfos.FirstOrDefault(x => x.ListOfIndexesNumberIsCountedOn.Contains(si - 1));
            if (numbLeft != null)
            {
                result *= numbLeft.Number;
                possibleTuples.Add((lineData.LineIndex, numbLeft.Number));
                states[2] = 1;
            }


            var numbRight = lineData.NumberInfos.FirstOrDefault(x => x.ListOfIndexesNumberIsCountedOn.Contains(si + 1));
            if (numbRight != null)
            {
                result *= numbRight.Number;
                possibleTuples.Add((lineData.LineIndex, numbRight.Number));
                states[3] = 1;
            }

            if (states.Sum(x => x) == 2)
            {
                var t1 = possibleTuples.First();
                var t2 = possibleTuples.Last();
                solutions.Add((t1.Item1, t1.Item2, t2.Item1, t2.Item2));
                sum += result;
            }
        }
        return sum;
    }

    void CalcPart1(String[] lines, Boolean print = false)
    {
        var lineDatas = ExtractLineData(lines);

        Console.WriteLine($"scanned {lineDatas.Count} line data");

        var sum = 0;
        var solutions = new List<(Int32, Int32)>();
        foreach (var ld in lineDatas)
        {
            sum += CheckCurrentLine(ld, solutions, print);
            if (!ld.IsTopLine)
                sum += CheckLineAbove(ld, lineDatas.First(x => x.LineIndex == ld.LineIndex - 1), solutions, print);
            if (!ld.IsBottomLine)
                sum += CheckLineBelow(ld, lineDatas.First(x => x.LineIndex == ld.LineIndex + 1), solutions, print);
        }
        Console.WriteLine($"Sum of parts: {sum}");
        if (print)
        {
            var sorted = solutions.OrderBy(x => x.Item1).ThenBy(y => y.Item2);
            foreach (var t in sorted)
            {
                Console.WriteLine($"[{t.Item1}]{t.Item2}");
            }
        }
    }

    private Int32 CheckLineAbove(LineData lineData, LineData lineDataAbove, List<(Int32, Int32)> solutions, Boolean print = false)
    {
        int sum = 0;
        if (!lineDataAbove.SymbolIndexes.Any())
            return sum;
        foreach (var ni in lineData.NumberInfos)
        {
            var possibleSymbolLocationsForMatch = Enumerable.Range(ni.NumberIndexStart - 1, (ni.NumberIndexEnd - ni.NumberIndexStart + 3)).ToArray();
            var symbolMatching = possibleSymbolLocationsForMatch.Any(x => lineDataAbove.SymbolIndexes.Contains(x));
            if (symbolMatching)
            {
                if (print) Console.WriteLine($"[{lineData.LineIndex}] {ni.Number} is part of engine.");
                sum += ni.Number;
                solutions.Add((lineData.LineIndex, ni.Number));
            }

        }
        return sum;
    }

    private Int32 CheckLineBelow(LineData lineData, LineData lineDataBelow, List<(Int32, Int32)> solutions, Boolean print = false)
    {
        int sum = 0;
        if (!lineDataBelow.SymbolIndexes.Any())
            return sum;
        foreach (var ni in lineData.NumberInfos)
        {
            var possibleSymbolLocationsForMatch = Enumerable.Range((ni.NumberIndexStart - 1), (ni.NumberIndexEnd - ni.NumberIndexStart + 3)).ToArray();
            var symbolMatching = possibleSymbolLocationsForMatch.Any(x => lineDataBelow.SymbolIndexes.Contains(x));
            if (symbolMatching)
            {
                if (print) Console.WriteLine($"[{lineData.LineIndex}] {ni.Number} is part of engine.");
                sum += ni.Number;
                solutions.Add((lineData.LineIndex, ni.Number));
            }

        }
        return sum;
    }

    private Int32 CheckCurrentLine(LineData lineData, List<(Int32, Int32)> solutions, Boolean print = false)
    {
        var sum = 0;
        if (!lineData.SymbolIndexes.Any())
            return sum;
        foreach (var ni in lineData.NumberInfos)
        {
            var symbolBefore = lineData.SymbolIndexes.Any(x => x == ni.NumberIndexStart - 1);
            var symbolAfter = lineData.SymbolIndexes.Any(x => x == ni.NumberIndexEnd + 1);
            if (symbolBefore || symbolAfter)
            {
                if (print) Console.WriteLine($"[{lineData.LineIndex}] {ni.Number} is part of engine.");
                sum += ni.Number;
                solutions.Add((lineData.LineIndex, ni.Number));
            }
        }
        return sum;
    }

    private List<LineData> ExtractLineData(String[] lines)
    {
        var lineDatas = new List<LineData>();
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var lineData = new LineData();
            lineData.Line = line;
            lineData.LineIndex = i;
            if (i == 0)
                lineData.IsTopLine = true;
            if (i == lines.Length - 1)
                lineData.IsBottomLine = true;

            var startingIndexOfNumber = -1;
            var currentNumberString = "";
            for (var j = 0; j < line.Length; j++)
            {
                // if (i == 131 && j >= 137)
                // {
                //     var charr = line[j];
                //     // Console.WriteLine("b");
                // }
                if (line[j] != '.')
                {
                    if (char.IsDigit(line[j]))
                    {
                        currentNumberString += line[j];
                        if (startingIndexOfNumber == -1)
                        {
                            startingIndexOfNumber = j;
                        }
                    }
                    else
                    {
                        lineData.SymbolIndexes.Add(j);
                        if (!String.IsNullOrWhiteSpace(currentNumberString))
                        {
                            var numberInfo = new NumberInfo();
                            numberInfo.NumberIndexStart = startingIndexOfNumber;
                            numberInfo.NumberIndexEnd = startingIndexOfNumber + currentNumberString.Length - 1;
                            numberInfo.ListOfIndexesNumberIsCountedOn = Enumerable.Range(numberInfo.NumberIndexStart, numberInfo.NumberIndexEnd - numberInfo.NumberIndexStart + 1).ToList();
                            numberInfo.Number = Int32.Parse(currentNumberString);
                            lineData.NumberInfos.Add(numberInfo);
                            startingIndexOfNumber = -1;
                            currentNumberString = "";
                        }
                    }
                }
                else
                {
                    if (startingIndexOfNumber != -1)
                    {
                        var numberInfo = new NumberInfo();
                        numberInfo.NumberIndexStart = startingIndexOfNumber;
                        numberInfo.NumberIndexEnd = startingIndexOfNumber + currentNumberString.Length - 1;
                        numberInfo.ListOfIndexesNumberIsCountedOn = Enumerable.Range(numberInfo.NumberIndexStart, numberInfo.NumberIndexEnd - numberInfo.NumberIndexStart + 1).ToList();
                        numberInfo.Number = Int32.Parse(currentNumberString);
                        lineData.NumberInfos.Add(numberInfo);
                        startingIndexOfNumber = -1;
                        currentNumberString = "";
                    }
                }
                if (j == line.Length - 1 && startingIndexOfNumber != -1)
                {
                    var numberInfo = new NumberInfo();
                    numberInfo.NumberIndexStart = startingIndexOfNumber;
                    numberInfo.NumberIndexEnd = startingIndexOfNumber + currentNumberString.Length - 1;
                    numberInfo.ListOfIndexesNumberIsCountedOn = Enumerable.Range(numberInfo.NumberIndexStart, numberInfo.NumberIndexEnd - numberInfo.NumberIndexStart + 1).ToList();
                    numberInfo.Number = Int32.Parse(currentNumberString);
                    lineData.NumberInfos.Add(numberInfo);
                    startingIndexOfNumber = -1;
                    currentNumberString = "";
                }
            }

            lineDatas.Add(lineData);
        }

        return lineDatas;
    }

    class LineData
    {
        public int LineIndex { get; set; }
        public Boolean IsTopLine { get; set; }
        public Boolean IsBottomLine { get; set; }
        public List<NumberInfo> NumberInfos { get; set; } = new List<NumberInfo>();
        public List<Int32> SymbolIndexes { get; set; } = new List<Int32>();
        public String Line { get; set; }

        public override String ToString()
        {
            return $"{LineIndex}|s:{String.Join(',', SymbolIndexes)}|n:{String.Join(',', NumberInfos.Select(x => x.Number))}";
        }
    }

    class NumberInfo
    {
        public int Number { get; set; }
        public int NumberIndexStart { get; set; }
        public int NumberIndexEnd { get; set; }
        public List<Int32> ListOfIndexesNumberIsCountedOn { get; set; } = new List<Int32>();
        public override String ToString()
        {
            return $"{Number}|i:{String.Join(',', ListOfIndexesNumberIsCountedOn)}";
        }
    }
}
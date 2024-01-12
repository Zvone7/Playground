#nullable enable

namespace Playground.aoc2023.t13.part1;

public class Task13Part1
{
    public void Main()
    {
        var fileName = "1.txt";// 
        fileName = "2.txt";//   61341 -
        // fileName = "3.txt";//  
        // fileName = "4.txt";//  
        // fileName = "5.txt";//  
        var fullFilePath = Path.Combine(Directory.GetCurrentDirectory(), "aoc2023", "t13", fileName);
        if (!File.Exists(fullFilePath))
        {
            throw new FileNotFoundException($"Can't find input file at {fullFilePath}!");
        }

        var lines = File.ReadAllLines(fullFilePath)
            .Where(
                x => !x.StartsWith("--")
                // &&
                // !string.IsNullOrWhiteSpace(x)
            ).ToArray();

        CalcPart(lines, false);
    }

    private void CalcPart(String[] lines, Boolean print = false)
    {
        var input = ParseInput(lines);

        var total = 0;
        for (var i = 0; i < input.Patterns.Count; i++)
        {
            var pattern = input.Patterns[i];
            var columnMirrors = TryFindMirroringColumn(pattern.Points, false);
            if (columnMirrors.Any())
            {
                Console.WriteLine($"[{i}] mirror are columns {String.Join(",", columnMirrors)}");
                total += CalcTotalForColumns(columnMirrors);
            }

            var rowMirrors = TryFindMirroringRow(pattern.Points, print);
            if (rowMirrors.Any())
            {
                Console.WriteLine($"[{i}] mirror are rows {String.Join(",", rowMirrors)}");
                total += CalcTotalForRows(rowMirrors);
            }

            Console.WriteLine();
        }


        Console.WriteLine($"Total: {total}");

    }

    Int32 CalcTotalForRows(List<Int32> members)
    {
        return members.Select(x => x * 100).Sum();
    }
    Int32 CalcTotalForColumns(List<Int32> members)
    {
        return members.Select(x => x).Sum();
    }

    List<Int32> TryFindMirroringRow(List<Point> points, Boolean print = false)
    {
        var maxX = points.Max(p => p.X);
        var maxY = points.Max(p => p.Y);
        // Dictionary<possibleMirrorColumnIndex, <y, difference_between_sums_around_mirror_columns>>
        var res = new List<(Int32 possibleMirrorRowIndex, List<(Int32 columnIndex, Int32 mirroringSumsDifference)> sumPerColumn)>();

        // 1 - maxY-1 because mirrorRow can never be edge column
        for (int possibleMirrorRowIndex = 1; possibleMirrorRowIndex < maxY - 1; possibleMirrorRowIndex++)
        {
            var sumsPerColumn = new List<(Int32 columnIndex, Int32 mirroringSumsDifference)>();

            for (var columnIndex = 0; columnIndex < maxX; columnIndex++)
            {
                var pointsBeforeMirrorIndex = points
                    .Where(p =>
                        p.X.Equals(columnIndex)
                        && p.Y < possibleMirrorRowIndex)
                    .ToList();
                var pointsAfterMirrorIndex = points
                    .Where(p =>
                        p.X.Equals(columnIndex)
                        && p.Y >= possibleMirrorRowIndex)
                    .ToList();
                var sumBefore = pointsBeforeMirrorIndex.Sum(p => p.Value);
                var sumAfter = pointsAfterMirrorIndex.Sum(p => p.Value);
                sumsPerColumn.Add((columnIndex, Math.Abs(sumBefore - sumAfter)));
            }
            res.Add((possibleMirrorRowIndex, sumsPerColumn));
        }

        if (print)
            foreach (var tuple in res)
            {
                Console.WriteLine($"-----");
                Console.WriteLine($"MirroringIndex: {tuple.possibleMirrorRowIndex}");
                foreach (var tuple2 in tuple.sumPerColumn)
                {
                    Console.WriteLine($"\t[{tuple2.columnIndex}] {tuple2.mirroringSumsDifference}");
                }
            }

        var resultingList = res
            .OrderBy(x => x.sumPerColumn.Sum(y => y.mirroringSumsDifference))
            .ToList();

        var possibleMirrorRows = new List<Int32>();
        foreach (var tuple in resultingList)
        {
            // Console.WriteLine($"Checking if it's mirrored for column {tuple.possibleMirrorColumnIndex}");
            var indexIsAPossibleRowMirror = true;
            for (var xIndex = 0; xIndex < maxX; xIndex++)
            {
                var line = String.Join("", points.Where(p => p.X.Equals(xIndex)).Select(p => p.Char).ToList());
                var linePart1 = line.Substring(0, tuple.possibleMirrorRowIndex);
                var linePart2 = line.Substring(tuple.possibleMirrorRowIndex + 1);
                // invert linePart2
                var linePart2Inverted = new String(linePart2.Reverse().ToArray());
                if (linePart1.Contains(linePart2Inverted))
                    continue;
                if (print) Console.WriteLine($"For {tuple.possibleMirrorRowIndex} no longer a match at x {xIndex}.");
                indexIsAPossibleRowMirror = false;
                break;
            }
            if (indexIsAPossibleRowMirror)
            {
                if (print) Console.WriteLine($"{tuple.possibleMirrorRowIndex} is a possible mirror");
                possibleMirrorRows.Add(tuple.possibleMirrorRowIndex);

            }
        }

        var members = possibleMirrorRows.Where((x, i) => i % 2 == 0).ToList();
        return members;
    }

    List<Int32> TryFindMirroringColumn(List<Point> points, Boolean print)
    {
        var maxX = points.Max(p => p.X);
        var maxY = points.Max(p => p.Y);
        // Dictionary<possibleMirrorColumnIndex, <y, difference_between_sums_around_mirror_columns>>
        var res = new List<(Int32 possibleMirrorColumnIndex, List<(Int32 rowIndex, Int32 mirroringSumsDifference)> sumPerRow )>();

        // 1 - maxX-1 because mirrorColumn can never be edge column
        for (int possibleMirrorColumnIndex = 1; possibleMirrorColumnIndex < maxX - 1; possibleMirrorColumnIndex++)
        {
            var sumsPerRow = new List<(Int32 rowIndex, Int32 mirroringSumsDifference)>();

            for (var rowIndex = 0; rowIndex < maxY; rowIndex++)
            {
                var pointsBeforeMirrorIndex = points
                    .Where(p =>
                        p.Y.Equals(rowIndex)
                        && p.X < possibleMirrorColumnIndex)
                    .ToList();
                var pointsAfterMirrorIndex = points
                    .Where(p =>
                        p.Y.Equals(rowIndex)
                        && p.X >= possibleMirrorColumnIndex)
                    .ToList();
                var sumBefore = pointsBeforeMirrorIndex.Sum(p => p.Value);
                var sumAfter = pointsAfterMirrorIndex.Sum(p => p.Value);
                sumsPerRow.Add((rowIndex, Math.Abs(sumBefore - sumAfter)));
            }
            res.Add((possibleMirrorColumnIndex, sumsPerRow));
        }

        if (print)
            foreach (var tuple in res)
            {
                Console.WriteLine($"-----");
                Console.WriteLine($"MirroringIndex: {tuple.possibleMirrorColumnIndex}");
                foreach (var tuple2 in tuple.sumPerRow)
                {
                    Console.WriteLine($"\t[{tuple2.rowIndex}] {tuple2.mirroringSumsDifference}");
                }
            }


        var resultingList = res
            .OrderBy(x => x.sumPerRow.Sum(y => y.mirroringSumsDifference))
            .ToList();


        var possibleMirrorColumns = new List<Int32>();
        foreach (var tuple in resultingList)
        {
            var indexIsAPossibleColumnMirror = true;
            for (var yIndex = 0; yIndex < maxY; yIndex++)
            {
                var line = String.Join("", points.Where(p => p.Y.Equals(yIndex)).Select(p => p.Char).ToList());
                var linePart1 = line.Substring(0, tuple.possibleMirrorColumnIndex);
                var linePart2 = line.Substring(tuple.possibleMirrorColumnIndex + 1);
                // invert linePart2
                var linePart2Inverted = new String(linePart2.Reverse().ToArray());
                if (linePart1.Contains(linePart2Inverted))
                    continue;
                // Console.WriteLine($"For {tuple.possibleMirrorColumnIndex} no longer a match at y {yIndex}.");
                indexIsAPossibleColumnMirror = false;
                break;
            }
            if (indexIsAPossibleColumnMirror)
            {
                if (print) Console.WriteLine($"{tuple.possibleMirrorColumnIndex} is a possible mirror");
                possibleMirrorColumns.Add(tuple.possibleMirrorColumnIndex);

            }
        }

        var members = possibleMirrorColumns.Where((x, i) => i % 2 == 0).ToList();
        return members;
    }

    Input ParseInput(String[] lines)
    {
        var input = new Input();
        input.Patterns.Add(new Pattern());
        Pattern pattern;
        pattern = input.Patterns.Last();
        var yHelper = 0;
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (String.IsNullOrWhiteSpace(line))
            {
                input.Patterns.Add(new Pattern());
                pattern = input.Patterns.Last();
                yHelper = 0;
                continue;
            }
            for (int j = 0; j < line.Length; j++)
            {
                var ch = line[j];
                pattern.Points.Add(new(j, yHelper, ch));
            }
            yHelper++;
        }
        Console.WriteLine($"data parsed");
        return input;
    }

}

public class Input
{
    public List<Pattern> Patterns { get; set; } = new();
}

public class Pattern
{
    public List<Point> Points { get; set; } = new();
}

public class Point
{
    public Point(
        Int32 x,
        Int32 y,
        Char ch
    )
    {
        Char = ch;
        X = x;
        Y = y;
        Value = ch == '.' ? 0 : 1;
    }
    public char Char { get; set; }
    public Int32 Value { get; set; }

    public Int32 X { get; set; }
    public Int32 Y { get; set; }

    public override String ToString()
    {
        return $"({X},{Y}) {Char}";
    }

}
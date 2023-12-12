#nullable enable
using System.Data;
using System.Text;

namespace Playground.aoc2023.t11.part1;

public class Task11Part1
{
    public void Main()
    {
        var fileName = "1.txt";//  374
        fileName = "2.txt";//  10173804
        // fileName = "3.txt";//  
        // fileName = "4.txt";//  
        // fileName = "5.txt";  //  
        var fullFilePath = Path.Combine(Directory.GetCurrentDirectory(), "aoc2023", "t11", fileName);
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
        var originalUniverse = ParseInput(lines);
        if (print) ShowUniverse(originalUniverse.Points);

        var expandedUniverse = ExpandUniverse(originalUniverse);
        if (print) ShowUniverse(expandedUniverse.Points);

        var galaxies = originalUniverse.Points
            .Where(x => x.IsGalaxy)
            .Select(p => new Galaxy(p.Name, p.X, p.Y))
            .ToList();

        var galaxyPairs = galaxies
            .SelectMany((g1, index1) => galaxies
                .Where((g2, index2) => index1 < index2)
                .Select(g2 => (g1, g2)))
            .ToList();

        Console.WriteLine($"Galaxy pairs created.");

        var total = galaxyPairs.Sum(gp => CalcDistanceBetweenGalaxies(
            gp.g1.OriginalIndex, gp.g1.X, gp.g1.Y,
            gp.g2.OriginalIndex, gp.g2.X, gp.g2.Y));

        Console.WriteLine($"Total: {total}");

    }

    private Int32 CalcDistanceBetweenGalaxies2(
        Galaxy g1, Galaxy g2,
        List<Int32> indexesOfEmptyRows,
        List<Int32> indexesOfEmptyColumns)
    {
        var res = Math.Abs(g1.X - g2.X) + Math.Abs(g1.Y - g2.Y);
        // Console.WriteLine($"[{g1Id}]({x1},{y1})\t -> [{g2Id}]({x2},{y2}) \t= {res}");
        return res;
    }

    private Int32 CalcDistanceBetweenGalaxies(String g1Id, Int32 x1, Int32 y1, String g2Id, Int32 x2, Int32 y2)
    {
        var res = Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
        // Console.WriteLine($"[{g1Id}]({x1},{y1})\t -> [{g2Id}]({x2},{y2}) \t= {res}");
        return res;
    }

    Input ExpandUniverse(Input input)
    {
        Console.WriteLine($"Started universe expansion.");
        var expandedInput = new Input();
        // to dictionary of key = x, value = list of points

        var rowsOfPointsOriginal = input
            .Points
            .GroupBy(p => p.Y)
            .ToDictionary(g => g.Key, g => g.ToList());

        var resTuple = FindExpandedIndexes(input.Points);
        var indexesOfEmptyRows = resTuple.indexesOfEmptyRows;
        var indexesOfEmptyColumns = resTuple.indexesOfEmptyColumns;


        var maxXOriginal = input.Points.Max(p => p.X);
        var maxYOriginal = input.Points.Max(p => p.Y);
        var expandedRows = new List<Point>();
        var expandedColumns = new List<Point>();

        var rowsAdded = 0;

        // add rows
        for (Int32 i = 0; i < maxYOriginal + 1; i++)
        {
            if (indexesOfEmptyRows.Contains(i))
            {
                var rowOfEmptyPoints = Enumerable.Range(0, maxXOriginal + 1)
                    .Select(j => new Point(j, i + rowsAdded, "."))
                    .ToList();
                expandedRows.AddRange(rowOfEmptyPoints);
                rowsAdded++;
            }
            var regularRow = input.Points.Where(g => g.Y.Equals(i)).ToList();
            var regularRowWithUpdatedY = regularRow.Select(p => new Point(p.X, p.Y + rowsAdded, p.Name, p.IsGalaxy)).ToList();
            expandedRows.AddRange(regularRowWithUpdatedY);
        }
        var all = expandedRows.OrderBy(p => p.X).ThenBy(p => p.Y).ToList();
        expandedInput.Points = all;
        Console.WriteLine($"rows expanded.");

        var maxXNew = expandedInput.Points.Max(p => p.X);
        var maxYNew = expandedInput.Points.Max(p => p.Y);
        var columnsAdded = 0;

        // add columns
        for (Int32 i = 0; i < maxXNew + 1; i++)
        {
            if (indexesOfEmptyColumns.Contains(i))
            {
                var columnOfEmptyPoints = Enumerable.Range(0, maxYNew + 1)
                    .Select(j => new Point(i + columnsAdded, j, "."))
                    .ToList();
                expandedColumns.AddRange(columnOfEmptyPoints);
                columnsAdded++;
            }
            var regularColumn = expandedInput.Points.Where(p => p.X.Equals(i)).ToList();
            var regularColumnWithUpdatedX = regularColumn.Select(p => new Point((p.X + columnsAdded), p.Y, p.Name, p.IsGalaxy)).ToList();
            expandedColumns.AddRange(regularColumnWithUpdatedX);
        }

        var allAgain = expandedColumns.OrderBy(p => p.X).ThenBy(p => p.Y).ToList();
        expandedInput.Points = allAgain;
        Console.WriteLine($"columns expanded.");

        return expandedInput;
    }

    (List<Int32> indexesOfEmptyRows, List<Int32> indexesOfEmptyColumns) FindExpandedIndexes(List<Point> points)
    {
        var rowsOfPointsOriginal =
            points
                .GroupBy(p => p.Y)
                .ToDictionary(g => g.Key, g => g.ToList());


        var indexesOfEmptyRows = new List<Int32>();
        foreach (var row in rowsOfPointsOriginal)
        {

            if (row.Value.All(p => p.Name == "."))
                indexesOfEmptyRows.Add(row.Key);
        }


        var columnsOfPointsOriginal =
            points
                .GroupBy(p => p.X)
                .ToDictionary(g => g.Key, g => g.ToList());
        var indexesOfEmptyColumns = new List<Int32>();

        foreach (var column in columnsOfPointsOriginal)
        {
            if (column.Value.All(p => p.Name == "."))
                indexesOfEmptyColumns.Add(column.Key);
        }

        return (indexesOfEmptyRows, indexesOfEmptyColumns);
    }

    public Input ParseInput(String[] lines)
    {
        var input = new Input();
        var points = new List<Point>();
        var galaxiesFound = 1;
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            for (Int32 j = 0; j < line.Length; j++)
            {
                var c = line[j];
                if (c == '.')
                    points.Add(new Point(j, i, "."));
                else if (c == '#')
                {
                    // points.Add(new Point(j, i, galaxiesFound.ToString(), true));
                    points.Add(new Point(j, i, "#", true));
                    galaxiesFound++;
                }
                else
                    throw new Exception($"ParseInput unhandled char {c}");
            }
        }

        input.Points = points;
        Console.WriteLine($"data parsed");
        return input;
    }

    public void ShowUniverse(List<Point> points)
    {

        var pointsGroupedByRow = points
            .GroupBy(p => p.Y)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var row in pointsGroupedByRow)
        {
            foreach (var p in row.Value)
            {
                Console.Write($"{p.Name} ");
            }
            Console.WriteLine();
        }

        Console.WriteLine("-----------");
    }
}

public class Input
{
    public List<Point> Points { get; set; } = new();
}

public class Point
{
    public Point(
        Int32 x,
        Int32 y,
        String ch,
        Boolean isGalaxy = false
    )
    {
        IsGalaxy = isGalaxy;
        Name = ch;
        X = x;
        Y = y;
    }
    public String Name { get; set; }
    public Boolean IsGalaxy { get; private set; }

    public Int32 X { get; set; }
    public Int32 Y { get; set; }

    public override String ToString()
    {
        return $"({X},{Y}) {Name} {(IsGalaxy ? "galaxy" : "")}";
    }

}

public class Galaxy
{
    public Galaxy(String originalIndex, Int32 x, Int32 y)
    {
        OriginalIndex = originalIndex;
        Id = $"({x},{y})";
        X = x;
        Y = y;
    }
    public String OriginalIndex;
    public String Id { get; set; }
    public Int32 X { get; set; }
    public Int32 Y { get; set; }

    public override String ToString()
    {
        return Id;
    }
}
#nullable enable
using System.Data;
using System.Text;

namespace Playground.aoc2023.t11.part2;

public class Task11Part2
{
    // private readonly Int64 _expansionMagnitude_ = 2;
    // private readonly Int64 _expansionMagnitude_ = 10;
    // private readonly Int64 _expansionMagnitude_ = 100;
    private readonly Int64 _expansionMagnitude_ = 1_000_000;
    public void Main()
    {
        var fileName = "1.txt";//  (2) 374, (10) 1030, (100) 8410, (1_000_000) 82000210 
        fileName = "2.txt";//  (2) 10173804, (10) 15248332,  (100) 72336772, (1_000_000) 634324905172
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

        var resTuple = FindExpandedIndexes(originalUniverse.Points);
        var indexesOfEmptyRows = resTuple.indexesOfEmptyRows;
        var indexesOfEmptyColumns = resTuple.indexesOfEmptyColumns;


        var galaxies = originalUniverse.Points
            .Where(x => x.IsGalaxy)
            .Select(p => new Galaxy(p.Name, p.X, p.Y))
            .ToList();

        var galaxyPairs = galaxies
            .SelectMany((g1, index1) => galaxies
                .Where((g2, index2) => index1 < index2)
                .Select(g2 => (g1, g2)))
            // .Where(g => g.g1.OriginalIndex.Equals("1") && g.g2.OriginalIndex.Equals("2"))
            // .Where(g => g.g1.OriginalIndex.Equals("1") && g.g2.OriginalIndex.Equals("7"))
            .ToList();

        Console.WriteLine($"Galaxy pairs created.");

        var total = galaxyPairs.Sum(gp => CalcDistanceBetweenGalaxies(
            gp.g1, gp.g2, indexesOfEmptyRows, indexesOfEmptyColumns, print));

        Console.WriteLine($"Total: {total}");

    }

    private Int64 CalcDistanceBetweenGalaxies(
        Galaxy g1, Galaxy g2,
        List<Int64> indexesOfEmptyRows,
        List<Int64> indexesOfEmptyColumns,
        Boolean print = false)
    {
        var emptyRowsBeforeG1 = indexesOfEmptyRows.Where(i => i < g1.Y).ToList();
        var emptyColumnsBeforeG1 = indexesOfEmptyColumns.Where(i => i < g1.X).ToList();
        var x1Expanded = g1.X + (emptyColumnsBeforeG1.Count > 0 ? (emptyColumnsBeforeG1.Count * _expansionMagnitude_ - emptyColumnsBeforeG1.Count) : 0);
        var y1Expanded = g1.Y + (emptyRowsBeforeG1.Count > 0 ? (emptyRowsBeforeG1.Count * _expansionMagnitude_ - emptyRowsBeforeG1.Count) : 0);

        var emptyRowsBeforeG2 = indexesOfEmptyRows.Where(i => i < g2.Y).ToList();
        var emptyColumnsBeforeG2 = indexesOfEmptyColumns.Where(i => i < g2.X).ToList();
        var x2Expanded = g2.X + (emptyColumnsBeforeG2.Count > 0 ? (emptyColumnsBeforeG2.Count * _expansionMagnitude_ - emptyColumnsBeforeG2.Count) : 0);
        var y2Expanded = g2.Y + (emptyRowsBeforeG2.Count > 0 ? (emptyRowsBeforeG2.Count * _expansionMagnitude_ - emptyRowsBeforeG2.Count) : 0);

        var res = Math.Abs(x1Expanded - x2Expanded) + Math.Abs(y1Expanded - y2Expanded);
        if (print) Console.WriteLine($"{g1}\t -> {g2} \t= {res}");
        return res;
    }

    (List<Int64> indexesOfEmptyRows, List<Int64> indexesOfEmptyColumns) FindExpandedIndexes(List<Point> points)
    {
        var rowsOfPointsOriginal =
            points
                .GroupBy(p => p.Y)
                .ToDictionary(g => g.Key, g => g.ToList());


        var indexesOfEmptyRows = new List<Int64>();
        foreach (var row in rowsOfPointsOriginal)
        {

            if (row.Value.All(p => p.Name == "."))
                indexesOfEmptyRows.Add(row.Key);
        }


        var columnsOfPointsOriginal =
            points
                .GroupBy(p => p.X)
                .ToDictionary(g => g.Key, g => g.ToList());
        var indexesOfEmptyColumns = new List<Int64>();

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
            for (var j = 0; j < line.Length; j++)
            {
                var c = line[j];
                if (c == '.')
                    points.Add(new Point(j, i, "."));
                else if (c == '#')
                {
                    points.Add(new Point(j, i, galaxiesFound.ToString(), true));
                    // points.Add(new Point(j, i, "#", true));
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
        Int64 x,
        Int64 y,
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

    public Int64 X { get; set; }
    public Int64 Y { get; set; }

    public override String ToString()
    {
        return $"({X},{Y}) {Name} {(IsGalaxy ? "galaxy" : "")}";
    }

}

public class Galaxy
{
    public Galaxy(String originalIndex, Int64 x, Int64 y)
    {
        OriginalIndex = originalIndex;
        Id = $"({x},{y})";
        X = x;
        Y = y;
    }
    public String OriginalIndex;
    public String Id { get; set; }
    public Int64 X { get; set; }
    public Int64 Y { get; set; }

    public override String ToString()
    {
        return $"{OriginalIndex}({X}, {Y})";
    }
}
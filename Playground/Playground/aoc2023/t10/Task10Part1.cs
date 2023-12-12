#nullable enable
using System.Data;
using System.Text;

namespace Playground.aoc2023.t10.part1;

public class Task10Part1
{
    readonly char _nodeIconF_ = '\u250F';// F
    readonly char _nodeIconJ_ = '\u251B';// J
    readonly char _nodeIconL_ = '\u2517';// L
    readonly char _nodeIcon7_ = '\u2513';// 7
    readonly char _nodeIconPipe_ = '\u2503';// -
    readonly char _nodeIconFlat_ = '\u2501';// |
    public void Main()
    {
        var fileName = "1.txt";//4, 1
        // fileName = "2.txt";//4, 1
        // fileName = "3.txt";//8, 1
        fileName = "4.txt";//6733, 459194-too high, 683 too high, 147 too low
        // fileName = "5.txt";
        var fullFilePath = Path.Combine(Directory.GetCurrentDirectory(), "aoc2023", "t10", fileName);
        if (!File.Exists(fullFilePath))
        {
            throw new FileNotFoundException($"Can't find input file at {fullFilePath}!");
        }

        var lines = File.ReadAllLines(fullFilePath)
            .Where(x => !x.StartsWith("#") &&
                        !String.IsNullOrWhiteSpace(x)).ToArray();

        CalcPart(lines, true);
        // Test();
    }

    private void CalcPart(String[] lines, Boolean print = false)
    {
        var input = ParseInput(lines);
        ConnectAnimalPoint(input);
        var graph = CreateGraph(input);
        Console.WriteLine($"Graph created.");
        // if (print) VisualizeGraph(graph);
        var graphReduced = graph.Where(x => x.Neighbours.Count > 1).ToList();

        Console.WriteLine($"Finding largest loop...");
        var largestClosedGraph = FindLargestClosedLoop(graphReduced);

        // if (print) VisualizeGraph(largestClosedGraph);

        var res = largestClosedGraph.Count / 2;

        Console.WriteLine($"Furthest distance from animal: {res}");

        var surface = CalculateArea2(graph, largestClosedGraph);
        
        Console.WriteLine($"inners: {surface}");
        
        if (print)
            VisualizeGraph2(
                largestClosedGraph,
                graph.Where(x => x.ch.Equals('x')).ToList()
            );
    }


    int CalculateArea2(List<Node> allNodes, List<Node> nodes)
    {
        var res = 0;

        // get all nodes from allNodes which are not in in nodes
        var allNotLoopedNodes = allNodes.Where(x => !nodes.Any(n => n.Id.Equals(x.Id))).ToList();
        foreach (var en in allNotLoopedNodes)
        {
            
            var charsRight = nodes.Where(n => n.ch != '.' && n.ch != '*' && n.X > en.X && n.Y.Equals(en.Y)).ToList();
            var charsBelow = nodes.Where(n => n.ch != '.' && n.ch != '*' && n.Y > en.Y && n.X.Equals(en.X)).ToList();
            if (en.Id.Equals("x9y7"))
            {

            }

            var isInsideVertically = DetermineAdditional(charsRight, computeVertically: true);
            // if (isInsideHorizontally.Equals(Position.unknown))
            var isInsideHorizontally = DetermineAdditional(charsBelow, computeVertically: false);


            if (isInsideVertically == Position.inside && isInsideHorizontally == Position.inside)
            {
                en.ch = '*';
                res++;
            }
            else if (isInsideVertically == Position.unknown && isInsideHorizontally == Position.unknown)
                throw new DataException($"Double unknown for {en.Id}");

        }
        return res;
    }

    private Position DetermineAdditional(List<Node> anglesRight, Boolean computeVertically = false)
    {
        var res = Position.outside;
        Int32 blockingWalls = 0;
        Boolean found7 = false;
        Boolean foundL = false;
        Boolean foundF = false;
        Boolean foundJ = false;
        foreach (var n in anglesRight)
        {
            if (computeVertically)
            {
                if (n.ch == _nodeIconPipe_)
                {
                    blockingWalls++;
                    continue;
                }

            }
            else
            {
                if (n.ch == _nodeIconFlat_)
                {
                    blockingWalls++;
                    continue;
                }
            }
            if (n.ch == _nodeIcon7_)
            {
                found7 = true;
                if (foundL)
                {
                    blockingWalls++;
                    foundL = false;
                    found7 = false;
                }
            }
            else if (n.ch == _nodeIconL_)
            {
                foundL = true;
                if (found7)
                {
                    blockingWalls++;
                    foundL = false;
                    found7 = false;
                }
            }
            else if (n.ch == _nodeIconF_)
            {
                foundF = true;
                if (foundJ)
                {
                    blockingWalls++;
                    foundF = false;
                    foundJ = false;
                }
            }
            else if (n.ch == _nodeIconJ_)
            {
                foundJ = true;
                if (foundF)
                {
                    blockingWalls++;
                    foundJ = false;
                    foundF = false;
                }
            }
        }

        if (Int32.IsOddInteger(blockingWalls))
            res = Position.inside;

        return res;
    }

    enum Position
    {
        outside = -1,
        unknown,
        inside
    }

    double CalculateArea(List<Node> nodes)
    {
        int n = nodes.Count;
        if (n < 3)
        {
            throw new ArgumentException("A polygon must have at least 3 vertices.");
        }

        double area = 0.0;

        // Find the centroid (average X and Y values)
        double centroidX = nodes.Average(node => node.X);
        double centroidY = nodes.Average(node => node.Y);

        // Sort the nodes based on their angles from the centroid
        nodes = nodes.OrderBy(node => Math.Atan2(node.Y - centroidY, node.X - centroidX)).ToList();


        for (int i = 0; i < n; i++)
        {
            int j = (i + 1) % n;// The next vertex

            area += ((nodes[i].X - 1) * (nodes[j].Y - 1)) - ((nodes[j].X - 1) * (nodes[i]).Y - 1);
        }

        return Math.Abs(area) / 2.0;
    }

    public List<Node> FindLargestClosedLoop(List<Node> nodes)
    {
        List<Node> largestLoop = new List<Node>();
        HashSet<Node> visited = new HashSet<Node>();
        var startNode = nodes.First(n => n.IsAnimal);
        List<Node> currentLoop = new List<Node>();

        if (FindClosedLoopDfs(startNode, startNode, currentLoop, visited) && currentLoop.Count > largestLoop.Count)
        {
            largestLoop = currentLoop;
        }

        return largestLoop;
    }

    bool FindClosedLoopDfs(Node startNode, Node currentNode, List<Node> currentLoop, HashSet<Node> visited)
    {
        Stack<Node> stack = new Stack<Node>();
        stack.Push(currentNode);

        while (stack.Count > 0)
        {
            var node = stack.Pop();
            visited.Add(node);
            currentLoop.Add(node);

            foreach (var neighbor in node.Neighbours)
            {
                if (neighbor == startNode && currentLoop.Count >= 3)
                {
                    // Found a closed loop
                    return true;
                }

                if (!visited.Contains(neighbor))
                {
                    stack.Push(neighbor);
                }
            }
        }

        currentLoop.Clear();
        visited.Clear();
        return false;
    }

    char GetNodeIcon(Char ch)
    {
        switch (ch)
        {
            case 'F':
                return _nodeIconF_;
            case 'J':
                return _nodeIconJ_;
            case 'L':
                return _nodeIconL_;
            case '7':
                return _nodeIcon7_;
            case '|':
                return _nodeIconPipe_;
            case '-':
                return _nodeIconFlat_;
            case '.': return '.';
            default: throw new Exception($"Unhandled char {ch}");
        }

    }

    List<Node> CreateGraph(Input input, Boolean skipNeighbours = false)
    {
        var nodes = input.Points.Select(p =>
        {
            var icon = GetNodeIcon(p.Char);
            var a = new Node(p.X, p.Y, icon);
            if (p.IsAnimal) a.IsAnimal = true;
            return a;
        }).ToList();
        foreach (var n in nodes)
        {
            var point = input.Points.First(p => p.X.Equals(n.X) && p.Y.Equals(n.Y));

            if (point.ConnectsRight)
            {
                var nodeRight = nodes.FirstOrDefault(n => n.X.Equals(point.X + 1) && n.Y.Equals(point.Y));
                if (nodeRight != null)
                    n.Neighbours.Add(nodeRight);
            }
            if (point.ConnectsLeft)
            {
                var nodeLeft = nodes.FirstOrDefault(n => n.X.Equals(point.X - 1) && n.Y.Equals(point.Y));
                if (nodeLeft != null)
                    n.Neighbours.Add(nodeLeft);
            }
            if (point.ConnectsUp)
            {
                var nodeUp = nodes.FirstOrDefault(n => n.X.Equals(point.X) && n.Y.Equals(point.Y - 1));
                if (nodeUp != null)
                    n.Neighbours.Add(nodeUp);
            }
            if (point.ConnectsDown)
            {
                var nodeDown = nodes.FirstOrDefault(n => n.X.Equals(point.X) && n.Y.Equals(point.Y + 1));
                if (nodeDown != null)
                    n.Neighbours.Add(nodeDown);
            }

        }
        return nodes;
    }

    public void ConnectAnimalPoint(Input input)
    {
        var animalPoint = input.Points.FirstOrDefault(p => p.IsAnimal);
        var pointAbove = input.Points.FirstOrDefault(p => p.X.Equals(animalPoint.X) && p.Y.Equals(animalPoint.Y - 1));
        var pointBelow = input.Points.FirstOrDefault(p => p.X.Equals(animalPoint.X) && p.Y.Equals(animalPoint.Y + 1));
        var pointLeft = input.Points.FirstOrDefault(p => p.X.Equals(animalPoint.X - 1) && p.Y.Equals(animalPoint.Y));
        var pointRight = input.Points.FirstOrDefault(p => p.X.Equals(animalPoint.X + 1) && p.Y.Equals(animalPoint.Y));

        if (pointLeft != null && pointRight != null)
            if (pointLeft.ConnectsRight && pointRight.ConnectsLeft)
            {
                animalPoint.ConnectsLeft = true;
                animalPoint.ConnectsRight = true;
                animalPoint.Char = '-';
            }
        if (pointAbove != null && pointBelow != null)
            if (pointAbove.ConnectsDown && pointBelow.ConnectsUp)
            {
                animalPoint.ConnectsUp = true;
                animalPoint.ConnectsDown = true;
                animalPoint.Char = '|';
            }
        if (pointAbove != null && pointLeft != null)
            if (pointAbove.ConnectsDown && pointLeft.ConnectsRight)
            {
                animalPoint.ConnectsUp = true;
                animalPoint.ConnectsLeft = true;
                animalPoint.Char = 'J';
            }
        if (pointAbove != null && pointRight != null)
            if (pointAbove.ConnectsDown && pointRight.ConnectsLeft)
            {
                animalPoint.ConnectsUp = true;
                animalPoint.ConnectsRight = true;
                animalPoint.Char = '7';
            }
        if (pointBelow != null && pointLeft != null)
            if (pointBelow.ConnectsUp && pointLeft.ConnectsRight)
            {
                animalPoint.ConnectsDown = true;
                animalPoint.ConnectsLeft = true;
                animalPoint.Char = 'L';
            }
        if (pointBelow != null && pointRight != null)
            if (pointBelow.ConnectsUp && pointRight.ConnectsLeft)
            {
                animalPoint.ConnectsDown = true;
                animalPoint.ConnectsRight = true;
                animalPoint.Char = 'F';
            }

    }

    public Input ParseInput(String[] lines)
    {
        var input = new Input();
        var points = new List<Point>();
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            for (int j = 0; j < line.Length; j++)
            {
                Point point = null;
                var ch = line[j];
                if (ch == 'S')
                    point = new Point(j, i, isAnimal: true);
                else
                {
                    point = ch switch
                    {
                        '-' => new Point(j, i, ch, connectsUp: false, connectsDown: false, connectsLeft: true, connectsRight: true),
                        '|' => new Point(j, i, ch, connectsUp: true, connectsDown: true, connectsLeft: false, connectsRight: false),
                        'L' => new Point(j, i, ch, connectsUp: true, connectsDown: false, connectsLeft: false, connectsRight: true),
                        'J' => new Point(j, i, ch, connectsUp: true, connectsDown: false, connectsLeft: true, connectsRight: false),
                        '7' => new Point(j, i, ch, connectsUp: false, connectsDown: true, connectsLeft: true, connectsRight: false),
                        'F' => new Point(j, i, ch, connectsUp: false, connectsDown: true, connectsLeft: false, connectsRight: true),
                        '.' => new Point(j, i, ch, false, false, false, false),
                        _ => throw new Exception($"Unhandled character {ch}")
                    };
                }

                points.Add(point);
            }

        }

        input.Points = points;
        return input;
    }

    public void VisualizeGraph(List<Node> nodes)
    {
        if (nodes.Count == 0)
        {
            Console.WriteLine("The graph is empty.");
            return;
        }

        int maxX = nodes.Max(node => node.X);
        int maxY = nodes.Max(node => node.Y);

        char[,] grid = new char[maxX, maxY];

        foreach (var node in nodes)
        {
            if (node.IsAnimal)
                grid[node.X - 1, node.Y - 1] = 'A';
            else
                grid[node.X - 1, node.Y - 1] = 'N';
        }

        for (int y = 0; y < maxY; y++)
        {
            for (int x = 0; x < maxX; x++)
            {
                if (grid[x, y] == 'N')
                {
                    Console.Write($"{nodes.First(n => n.X.Equals(x + 1) && n.Y.Equals(y + 1)).ch}");
                }
                else if (grid[x, y] == 'A')
                {
                    Console.Write("A");
                }
                else
                {
                    Console.Write(".");// '.' represents an empty cell
                }
            }
            Console.WriteLine();
        }

        Console.WriteLine("-------------");
    }


    void VisualizeGraph2(List<Node> nodes, List<Node> insideNodes)
    {
        if (nodes.Count == 0)
        {
            Console.WriteLine("The graph is empty.");
            return;
        }

        int maxX = nodes.Max(node => node.X);
        int maxY = nodes.Max(node => node.Y);

        char[,] grid = new char[maxX, maxY];

        foreach (var node in nodes)
        {
            if (node.IsAnimal)
                grid[node.X - 1, node.Y - 1] = 'A';
            else
                grid[node.X - 1, node.Y - 1] = 'N';
        }

        for (int y = 0; y < maxY; y++)
        {
            for (int x = 0; x < maxX; x++)
            {
                if (grid[x, y] == 'N')
                {
                    Console.Write($"{nodes.First(n => n.X.Equals(x + 1) && n.Y.Equals(y + 1)).ch}");
                }
                else if (grid[x, y] == 'A')
                {
                    Console.Write("A");
                }
                else
                {
                    var innerNode = insideNodes.FirstOrDefault(n => n.X.Equals(x + 1) && n.Y.Equals(y + 1));
                    if (innerNode != null)
                        Console.Write(innerNode.ch);
                    else
                        Console.Write(".");
                }
            }
            Console.WriteLine();
        }

        Console.WriteLine("-------------");
    }

}

public class Input
{
    public List<Point> Points { get; set; } = new();
}

public class Node
{
    public Node(Int32 x, Int32 y)
    {
        X = x;
        Y = y;
        Id = $"x{X}y{Y}";
    }
    public Node(Int32 x, Int32 y, char c)
    {
        X = x;
        Y = y;
        ch = c;
        Id = $"x{X}y{Y}";
    }
    public String Id { get; set; }
    public Int32 X { get; set; }
    public Int32 Y { get; set; }
    public char ch { get; set; }
    public Boolean IsAnimal { get; set; }
    public List<Node> Neighbours { get; set; } = new();

    public override String ToString()
    {
        return $"{Id} {ch}";
    }
}

public class Point
{
    public Point(
        Int32 x,
        Int32 y,
        Boolean isAnimal = false
    )
    {
        IsAnimal = isAnimal;
        X = x + 1;
        Y = y + 1;
    }
    public Point(
        Int32 x,
        Int32 y,
        char ch,
        Boolean connectsUp,
        Boolean connectsDown,
        Boolean connectsLeft,
        Boolean connectsRight
    )
    {
        Char = ch;
        ConnectsUp = connectsUp;
        ConnectsDown = connectsDown;
        ConnectsLeft = connectsLeft;
        ConnectsRight = connectsRight;
        X = x + 1;
        Y = y + 1;
    }
    public char Char { get; set; }
    public Boolean ConnectsUp { get; set; }
    public Boolean ConnectsDown { get; set; }
    public Boolean ConnectsLeft { get; set; }
    public Boolean ConnectsRight { get; set; }
    public Boolean IsAnimal { get; private set; }

    public Int32 X { get; set; }
    public Int32 Y { get; set; }

    public override String ToString()
    {
        // return $"({X},{Y}) up: {ConnectsUp}| down: {ConnectsDown}| left: {ConnectsLeft}| right: {ConnectsRight}| {(IsAnimal ? "animal" : "")}";
        return $"({X},{Y}) {(IsAnimal ? "animal" : "")} {(ConnectsUp ? "up" : "")} {(ConnectsDown ? "down" : "")} {(ConnectsLeft ? "left" : "")} {(ConnectsRight ? "right" : "")}";
    }

}
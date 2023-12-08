#nullable enable
using System.Data;

namespace Playground.aoc2023.t8.part1;

public class Task8Part1
{
    public void Main()
    {
        var fileName = "1.txt";
        fileName = "2.txt";
        fileName = "3.txt";
        var fullFilePath = Path.Combine(Directory.GetCurrentDirectory(), "aoc2023", "t8", fileName);
        if (!File.Exists(fullFilePath))
        {
            throw new FileNotFoundException($"Can't find input file at {fullFilePath}!");
        }

        var lines = File.ReadAllLines(fullFilePath)
            .Where(x => !x.StartsWith("#") &&
                        !String.IsNullOrWhiteSpace(x)).ToArray();

        CalcPart(lines, true);// 2 && 6 && 13207
    }

    private void CalcPart(String[] lines, Boolean print = false)
    {
        var input = ParseInput(lines);

        var currentNode = input.Nodes.FirstOrDefault(x => x.Name.Equals("AAA"));
        if (currentNode == null)
        {
            throw new DataException("No AAA node");
        }
        var setsDone = 0;
        while (currentNode.Name != "ZZZ")
        {
            foreach (var instruction in input.Instructions)
            {
                if (instruction == 'L')
                    currentNode = currentNode.Left;
                else if (instruction == 'R')
                    currentNode = currentNode.Right;
                else
                    throw new Exception("unknown instruction");

                if (print) Console.WriteLine($"[{setsDone}]Moved to {currentNode.Name}");
            }
            setsDone++;
        }

        Console.WriteLine($"Total instructions: {setsDone * input.Instructions.Length}");

    }


    public Input ParseInput(String[] lines)
    {
        var input = new Input();
        var nodes = new List<Node>();
        foreach (var line in lines)
        {
            if (!line.Contains("="))
                input.Instructions = line.Replace(" ", "");
            else
            {
                var nodeName = line.Split("=")[0].Replace(" ", "");
                var conn = line.Split("=")[1].Replace(" ", "")
                    .Replace("(", "")
                    .Replace(")", "");
                var connLeft = conn.Split(",")[0];
                var connRight = conn.Split(",")[1];
                nodes.Add(new(nodeName, connLeft, connRight));
            }

        }

        while (nodes.Any(x => !x.LeftNodeIsSet() || !x.RightNodeIsSet()))
        {
            foreach (var n in nodes.Where(x => !x.LeftNodeIsSet() || !x.RightNodeIsSet()))
            {
                var r = nodes.FirstOrDefault(x => x.Name.Equals(n.ConnRight));
                if (r == null)
                    throw new Exception($"No node found for instruction {n.ConnRight}");

                var l = nodes.FirstOrDefault(x => x.Name.Equals(n.ConnLeft));
                if (l == null)
                    throw new Exception($"No node found for instruction {n.ConnLeft}");

                n.ConnectLeft(l);
                n.ConnectRight(r);
            }
        }

        input.Nodes = nodes;
        return input;
    }


}

public class Input
{
    public String Instructions { get; set; }
    public List<Node> Nodes { get; set; } = new();
}

public class Node
{
    public String Name { get; }
    public String ConnLeft { get; }
    public String ConnRight { get; }
    public Node? Left { get; private set; }
    public Node? Right { get; private set; }
    public Node(String name, String connLeft, String connRight)
    {
        Name = name;
        ConnLeft = connLeft;
        ConnRight = connRight;
    }

    public override String ToString()
    {
        return $"{Name} l:{Left?.Name} r:{Right?.Name}";
    }

    public void ConnectLeft(Node? node)
    {
        Left = node;
    }

    public void ConnectRight(Node? node)
    {
        Right = node;
    }

    public Boolean LeftNodeIsSet()
    {
        return Left != null;
    }
    public Boolean RightNodeIsSet()
    {
        return Right != null;
    }

    public Node DeepCopy()
    {
        var copy= new Node(Name, ConnLeft, ConnRight);
        copy.ConnectLeft(Left);
        copy.ConnectRight(Right);

        return copy;
    }
}
using System.Text.RegularExpressions;

namespace Playground.aoc2023.t2;

public class Task2
{
    public const Int32 MaxRedCubes = 12;
    public const Int32 MaxBlueCubes = 14;
    public const Int32 MaxGreenCubes = 13;
    public void Main()
    {
        var fileName = "p1_1.txt";
        fileName = "p1_2.txt";
        var fullFilePath = Path.Combine(Directory.GetCurrentDirectory(), "aoc2023", "t2", fileName);
        if (!File.Exists(fullFilePath))
        {
            throw new FileNotFoundException($"Can't find input file at {fullFilePath}!");
        }

        var lines = File.ReadAllLines(fullFilePath);

        // CalcPart1(lines);
        CalcPart2(lines, false);
    }


    void CalcPart1(String[] lines)
    {
        var gameInfos = ParseGameInfos(lines);

        var fittableGameInfos = new List<GameInfo>();
        foreach (var gameInfo in gameInfos)
        {
            var gameWithMoreThenRedsAllowed = gameInfo.CubeInfos.FirstOrDefault(x => x.RedCubes > MaxRedCubes);
            var gameWithMoreThenBluesAllowed = gameInfo.CubeInfos.FirstOrDefault(x => x.BlueCubes > MaxBlueCubes);
            var gameWithMoreThenGreensAllowed = gameInfo.CubeInfos.FirstOrDefault(x => x.GreenCubes > MaxGreenCubes);
            if (gameWithMoreThenRedsAllowed == null &&
                gameWithMoreThenBluesAllowed == null &&
                gameWithMoreThenGreensAllowed == null)
            {
                fittableGameInfos.Add(gameInfo);
            }
        }
        var sumOfFitableIds = fittableGameInfos.Sum(x => x.GameId);
        Console.WriteLine($"game ids that could have happened: {String.Join(",", fittableGameInfos.Select(x => x.GameId))}\n" +
                          $"sum of ids: {sumOfFitableIds}");
    }

    void CalcPart2(String[] lines, Boolean print)
    {
        var gameInfos = ParseGameInfos(lines);

        var sum = 0;
        foreach (var gameInfo in gameInfos)
        {
            var minReds = gameInfo.CubeInfos.Max(x => x.RedCubes);
            var minBlues = gameInfo.CubeInfos.Max(x => x.BlueCubes);
            var minGreens = gameInfo.CubeInfos.Max(x => x.GreenCubes);

            var powerOfGame = minReds * minBlues * minGreens;
            if (print) Console.WriteLine($"Power of game {gameInfo.GameId}: {powerOfGame}");
            sum += powerOfGame;
        }
        Console.WriteLine($"Sum of powers: {sum}");
    }

    List<GameInfo> ParseGameInfos(String[] lines)
    {
        var gameInfos = new List<GameInfo>();
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var gameInfo = new GameInfo();
            var gameInfoStart = line.Split(":")[0];
            var gameInfoCubeData = line.Split(":")[1];
            gameInfo.GameId = Int32.Parse(gameInfoStart.Split("Game ")[1]);
            var cubeInfos = gameInfoCubeData.Split("; ");
            gameInfo.CubeInfos = new List<CubeInfo>();
            foreach (var cubeInfo in cubeInfos)
            {
                var colors = cubeInfo.Split(",");
                var ci = new CubeInfo();
                foreach (var c in colors)
                {
                    if (c.ToLower().Contains("red"))
                    {
                        var numb = int.Parse(c.Split(" red")[0]);
                        ci.RedCubes = numb;
                    }
                    else if (c.ToLower().Contains("green"))
                    {
                        var numb = int.Parse(c.Split(" green")[0]);
                        ci.GreenCubes = numb;
                    }
                    else if (c.ToLower().Contains("blue"))
                    {
                        var numb = int.Parse(c.Split(" blue")[0]);
                        ci.BlueCubes = numb;
                    }
                }
                gameInfo.CubeInfos.Add(ci);
            }
            gameInfos.Add(gameInfo);
        }

        return gameInfos;
    }

    class GameInfo
    {
        public int GameId { get; set; }
        public List<CubeInfo> CubeInfos { get; set; }
    }

    class CubeInfo
    {
        public int RedCubes { get; set; }
        public int GreenCubes { get; set; }
        public int BlueCubes { get; set; }
    }
}
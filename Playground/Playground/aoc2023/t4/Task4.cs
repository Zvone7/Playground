using System.ComponentModel;
using System.Diagnostics;

namespace Playground.aoc2023.t3;

public class Task4
{
    public void Main()
    {
        var fileName = "1.txt";
        // fileName = "2.txt";
        var fullFilePath = Path.Combine(Directory.GetCurrentDirectory(), "aoc2023", "t4", fileName);
        if (!File.Exists(fullFilePath))
        {
            throw new FileNotFoundException($"Can't find input file at {fullFilePath}!");
        }

        var lines = File.ReadAllLines(fullFilePath);

        // CalcPart1(lines, true); // 13 && 21558
        CalcPart2(lines, true); // 30 && 10_425_665
    }

    private void CalcPart2(String[] lines, Boolean print = false)
    {
        var gameDatas = ExtractLineData(lines);
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var stopwatchMain = new Stopwatch();
        stopwatchMain.Start();

        var scratchcards = gameDatas.Select(gm => new Scratchcard(gm.CardIndex)).ToList();

        while (scratchcards.Any(x => !x.Processed))
        {
            var unprocessedScratchcards = scratchcards
                .Where(x => !x.Processed)
                .OrderBy(x => x.Index)
                .ToList();
            if (print)
            {
                Console.WriteLine("-------state:");
                PrintState(scratchcards, stopwatch);
            }
            foreach (var usc in unprocessedScratchcards)
            {
                var gm = gameDatas.FirstOrDefault(x => x.CardIndex.Equals(usc.Index));
                if (gm == null)
                {
                    Console.WriteLine($"Can't find game with index {usc.Index}");
                    usc.Processed = true;
                    continue;
                }
                var totalWins = gm.WinningNumbers.Sum(wn => gm.GameNumbers.Count(x => x.Equals(wn)));
                usc.Processed = true;
                if (totalWins <= 0)
                    continue;
                for (var j = 1; j <= totalWins; j++)
                {
                    scratchcards.Add(new Scratchcard(gm.CardIndex + j));
                }
            }
        }

        if (print)
        {
            PrintState(scratchcards, stopwatchMain);
        }
        Console.WriteLine($"Total winnings: {scratchcards.Count}");
    }

    private void PrintState(List<Scratchcard> scratchcards, Stopwatch stopwatch)
    {

        // var groupedScratchards = scratchcards
        //     .GroupBy(x => x.Index)
        //     .Select(x => new { Index = x.Key, Count = x.Count() });
        //
        // foreach (var gsc in groupedScratchards)
        // {
        //     Console.WriteLine($"{gsc.Index}:{gsc.Count}");
        // }
        var processed = scratchcards.Count(x => x.Processed);
        Console.WriteLine($"Processed: {processed}/{scratchcards.Count} = {(float)(processed / (float)scratchcards.Count) * 100}%");
        Console.WriteLine($"Time elapsed: {stopwatch.ElapsedMilliseconds}ms");
        stopwatch.Restart();
    }

    private void CalcPart1(String[] lines, Boolean print = false)
    {
        var gameDatas = ExtractLineData(lines);

        foreach (var gm in gameDatas)
        {
            var gameResult = 0;
            foreach (var gmn in gm.GameNumbers)
            {
                if (gm.WinningNumbers.Contains(gmn))
                {
                    if (gameResult == 0)
                        gameResult = 1;
                    else
                        gameResult *= 2;
                }
            }
            gm.TotalWin = gameResult;
        }

        if (print)
        {
            foreach (var gm in gameDatas.Where(gm => gm.TotalWin > 0))
            {
                Console.WriteLine($"Card [{gm.CardIndex}] wins {gm.TotalWin} points.");
            }
        }
        Console.WriteLine($"Total winnings: {gameDatas.Sum(x => x.TotalWin)}");
    }


    private List<GameData> ExtractLineData(String[] lines)
    {
        var gameDatas = new List<GameData>();
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var gameData = new GameData();
            gameData.CardIndex = i + 1;
            var numberData = line.Split(":")[1];
            var winningNumbersAllString = numberData.Split("|")[0];
            var winningNumbersStrings = winningNumbersAllString.Split(" ");
            var winningNumbers = new List<Int32>();
            foreach (var wns in winningNumbersStrings)
            {
                var r = Int32.TryParse(wns, out var n);
                if (r)
                    winningNumbers.Add(n);
            }
            gameData.WinningNumbers = winningNumbers.ToArray();
            var gameNumbersAllString = numberData.Split("|")[1];
            var gameNumbersStrings = gameNumbersAllString.Split(" ");
            var gameNumbers = new List<Int32>();
            foreach (var wns in gameNumbersStrings)
            {
                var r = Int32.TryParse(wns, out var n);
                if (r)
                    gameNumbers.Add(n);
            }
            gameData.GameNumbers = gameNumbers.ToArray();
            gameDatas.Add(gameData);
        }

        return gameDatas;
    }

    class GameData
    {
        public Int32 CardIndex { get; set; }
        public Int32[] WinningNumbers { get; set; }
        public Int32[] GameNumbers { get; set; }
        public Int32 TotalWin { get; set; }
    }

    class Scratchcard
    {
        public Int32 Index { get; set; }
        public Boolean Processed { get; set; }
        public Scratchcard(Int32 index, Boolean processed = false)
        {
            Index = index;
            Processed = processed;
        }
    }
}
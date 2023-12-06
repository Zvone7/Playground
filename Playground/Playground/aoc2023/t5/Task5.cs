using System.ComponentModel;
using System.Data;
using System.Diagnostics;

namespace Playground.aoc2023.t3;

public class Task5
{
    public void Main()
    {
        var fileName = "1.txt";
        fileName = "2.txt";
        var fullFilePath = Path.Combine(Directory.GetCurrentDirectory(), "aoc2023", "t5", fileName);
        if (!File.Exists(fullFilePath))
        {
            throw new FileNotFoundException($"Can't find input file at {fullFilePath}!");
        }

        var lines = File.ReadAllLines(fullFilePath);

        // CalcPart1(lines, true);// 35 && 535088217
        CalcPart2(lines, true);// 46 && 51399228 (took 100 mins)
    }


    private void CalcPart2(String[] lines, Boolean print = false)
    {
        var sw = new Stopwatch();
        sw.Start();
        if (print) Console.WriteLine("Started collecting input.");
        var input = ExtractLineDataPt2(lines);
        if (print) Console.WriteLine($"Finished collecting input. {sw.Elapsed.Seconds}s");
        // PrintHelp(input, print);
        if (print) Console.WriteLine("Started calc.");
        var seedInfos = CalcSeedIndexes2(input, print);
        PrintHelp2(seedInfos.ToList(), print);
    }

    private void CalcPart1(String[] lines, Boolean print = false)
    {
        var input = ExtractLineData(lines);
        // PrintHelp(input, print);
        var seedInfos = CalcSeedIndexes(input, print);
        PrintHelp2(seedInfos, print);
    }

    private HashSet<SeedInfo> CalcSeedIndexes2(Input2 input, Boolean print)
    {
        var sw = new Stopwatch();
        sw.Start();
        var seedInfos = new HashSet<SeedInfo>();
        var seedPairs = input.Seeds.ToArray();
        var seedsCount = seedPairs.Length;
        var printIncrement = 1;
        if (print)
        {
            Console.WriteLine($"Doing calculation for {seedsCount} seeds.");
        }
        for (long ll = 0; ll < seedPairs.Length; ll++)
        {
            var seedPair = seedPairs[ll];
            if (print && (ll % printIncrement == 0))
            {
                Console.WriteLine($"Calculated {ll}/{seedsCount} = {ll / (float)seedsCount * 100}%");
                Console.WriteLine($"Time elapsed: {sw.Elapsed.Minutes}m {sw.Elapsed.Seconds}s");
                sw.Restart();
            }
            if (print) Console.WriteLine($"Checking out seed pair with length {seedPair.Item2}");

            var printIncrement2 = 0.01 * seedPair.Item2;
            // printIncrement2 = 2;
            for (int i = 0; i < seedPair.Item2; i++)
            {

                if (print && i % printIncrement2 == 0)
                {
                    Console.WriteLine($"SP[{ll}] Calculated {i}/{seedPair.Item2} = {ll / (float)seedPair.Item2 * 100}%");
                }
                var seed = seedPair.Item1 + i;
                var si = new SeedInfo();

                si.SeedIndex = seed;
                si.SoilIndex = CalculateMatchingIndex2(input, input.SeedToSoil, seed);
                si.FertIndex = CalculateMatchingIndex2(input, input.SoilToFert, si.SoilIndex);
                si.WaterIndex = CalculateMatchingIndex2(input, input.FertToWater, si.FertIndex);
                si.LightIndex = CalculateMatchingIndex2(input, input.WaterToLight, si.WaterIndex);
                si.TempIndex = CalculateMatchingIndex2(input, input.LightToTemp, si.LightIndex);
                si.HumidIndex = CalculateMatchingIndex2(input, input.TempToHumid, si.TempIndex);
                si.LocIndex = CalculateMatchingIndex2(input, input.HumidToLoc, si.HumidIndex);

                if (seedInfos.FirstOrDefault(x => x.LocIndex < si.LocIndex) == null)
                    seedInfos.Add(si);
            }
        }

        return seedInfos;
    }


    private List<SeedInfo> CalcSeedIndexes(Input input, Boolean print)
    {
        var sw = new Stopwatch();
        sw.Start();
        var seedInfos = new List<SeedInfo>();
        var seeds = input.Seeds.ToArray();
        var seedsCount = seeds.Length;
        var printIncrement = 0.01 * seedsCount;
        if (print)
        {
            Console.WriteLine($"Doing calculation for {seedsCount} seeds.");
        }
        for (long ll = 0; ll < seeds.Length; ll++)
        {
            var seed = seeds[ll];
            if (print && (ll % printIncrement == 0))
            {
                Console.WriteLine($"Calculated {ll}/{seedsCount} = {ll / (float)seedsCount * 100}%");
                Console.WriteLine($"Time elapsed: {sw.Elapsed.Minutes}m {sw.Elapsed.Seconds}s");
                sw.Restart();
            }
            var si = new SeedInfo();
            si.SeedIndex = seed;

            si.SoilIndex = CalculateMatchingIndex(input, input.SeedToSoil, seed);
            si.FertIndex = CalculateMatchingIndex(input, input.SoilToFert, si.SoilIndex);
            si.WaterIndex = CalculateMatchingIndex(input, input.FertToWater, si.FertIndex);
            si.LightIndex = CalculateMatchingIndex(input, input.WaterToLight, si.WaterIndex);
            si.TempIndex = CalculateMatchingIndex(input, input.LightToTemp, si.LightIndex);
            si.HumidIndex = CalculateMatchingIndex(input, input.TempToHumid, si.TempIndex);
            si.LocIndex = CalculateMatchingIndex(input, input.HumidToLoc, si.HumidIndex);

            seedInfos.Add(si);
        }

        return seedInfos;
    }

    private static Int64 CalculateMatchingIndex(Input input, List<SourceToDestinationInfo> stds, Int64 seedIndex)
    {
        long wantedIndex = -1;
        foreach (var std in stds)
        {
            var sourceRangeEnd = std.SourceRangeStart + std.RangeLength - 1;
            if (seedIndex >= std.SourceRangeStart && seedIndex <= sourceRangeEnd)
            {
                // 98 - 98 = 0
                var differenceFromSeedToSourceIndexStart = seedIndex - std.SourceRangeStart;
                var wIndex = std.DestinationRangeStart + differenceFromSeedToSourceIndexStart;
                wantedIndex = wIndex;
            }
        }
        if (wantedIndex == -1)
        {
            wantedIndex = seedIndex;
        }

        return wantedIndex;
    }

    private static Int64 CalculateMatchingIndex2(Input2 input, List<SourceToDestinationInfo> stds, Int64 seedIndex)
    {
        long wantedIndex = -1;
        foreach (var std in stds)
        {
            // if seed were 98, soil would be 50
            var sourceRangeEnd = std.SourceRangeStart + std.RangeLength - 1;
            if (seedIndex >= std.SourceRangeStart && seedIndex <= sourceRangeEnd)
            {
                // 98 - 98 = 0
                var differenceFromSeedToSourceIndexStart = seedIndex - std.SourceRangeStart;
                var wIndex = std.DestinationRangeStart + differenceFromSeedToSourceIndexStart;
                wantedIndex = wIndex;
            }
        }
        if (wantedIndex == -1)
        {
            wantedIndex = seedIndex;
        }

        return wantedIndex;
    }

    void PrintHelp(Input input, Boolean print = false)
    {
        if (print)
        {
            Console.WriteLine($"Seeds: {String.Join(',', input.Seeds)}");

            Console.WriteLine("----");
            Console.WriteLine("seed to soil");
            foreach (var sourceToDestination in input.SeedToSoil)
            {
                Console.WriteLine(sourceToDestination);
            }
            Console.WriteLine("----");
            Console.WriteLine("soil to fertilizer");
            foreach (var sourceToDestination in input.SoilToFert)
            {
                Console.WriteLine(sourceToDestination);
            }
            Console.WriteLine("----");
            Console.WriteLine("fertilizer to water");
            foreach (var sourceToDestination in input.FertToWater)
            {
                Console.WriteLine(sourceToDestination);
            }
            Console.WriteLine("----");
            Console.WriteLine("water to light");
            foreach (var sourceToDestination in input.WaterToLight)
            {
                Console.WriteLine(sourceToDestination);
            }
            Console.WriteLine("----");
            Console.WriteLine("light to temperature");
            foreach (var sourceToDestination in input.LightToTemp)
            {
                Console.WriteLine(sourceToDestination);
            }
            Console.WriteLine("----");
            Console.WriteLine("temperature to humidity");
            foreach (var sourceToDestination in input.TempToHumid)
            {
                Console.WriteLine(sourceToDestination);
            }
            Console.WriteLine("----");
            Console.WriteLine("humidity to location");
            foreach (var sourceToDestination in input.HumidToLoc)
            {
                Console.WriteLine(sourceToDestination);
            }
        }
    }

    void PrintHelp2(List<SeedInfo> seedInfos, Boolean print = false)
    {
        var sorted = seedInfos.OrderByDescending(x => x.LocIndex).ToList();
        // if (print)
        // {
        //     foreach (var si in sorted)
        //     {
        //         Console.WriteLine(si);
        //     }
        // }

        Console.WriteLine($"Lowest: {sorted.Last().LocIndex}");
    }

    private Input ExtractLineData(String[] lines)
    {
        var input = new Input();
        Boolean collectingSeedToSoil = false;
        Boolean collectingSoilToFert = false;
        Boolean collectingFertToWater = false;
        Boolean collectingWaterToLight = false;
        Boolean collectingLightToTemp = false;
        Boolean collectingTempToHumid = false;
        Boolean collectingHumidToLoc = false;

        var linesTemp = new List<String>();
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (String.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
            {
                continue;
            }
            else if (line.StartsWith("seeds"))
            {
                var seedsString = line.Split("seeds:")[1];
                var seedsStrings = seedsString.Split(" ");
                foreach (var s in seedsStrings)
                {
                    var r = Int64.TryParse(s, out var sn);
                    if (r)
                        input.Seeds.Add(sn);
                }
            }
            else if (line.StartsWith("seed-to-soil map:"))
            {
                collectingSeedToSoil = true;
            }
            else if (line.StartsWith("soil-to-fertilizer map:"))
            {
                input.SeedToSoil = ParseMap(linesTemp.ToArray());
                linesTemp.Clear();
                collectingSeedToSoil = false;
                collectingSeedToSoil = true;
            }
            else if (line.StartsWith("fertilizer-to-water map:"))
            {
                input.SoilToFert = ParseMap(linesTemp.ToArray());
                linesTemp.Clear();
                collectingSeedToSoil = false;
                collectingSoilToFert = true;
            }
            else if (line.StartsWith("water-to-light map:"))
            {
                input.FertToWater = ParseMap(linesTemp.ToArray());
                linesTemp.Clear();
                collectingSoilToFert = false;
                collectingWaterToLight = true;
            }
            else if (line.StartsWith("light-to-temperature map:"))
            {
                input.WaterToLight = ParseMap(linesTemp.ToArray());
                linesTemp.Clear();
                collectingWaterToLight = false;
                collectingLightToTemp = true;
            }
            else if (line.StartsWith("temperature-to-humidity map:"))
            {
                input.LightToTemp = ParseMap(linesTemp.ToArray());
                linesTemp.Clear();
                collectingLightToTemp = false;
                collectingTempToHumid = true;
            }
            else if (line.StartsWith("humidity-to-location map:"))
            {
                input.TempToHumid = ParseMap(linesTemp.ToArray());
                linesTemp.Clear();
                collectingTempToHumid = false;
                collectingHumidToLoc = true;
            }
            else
            {
                if (collectingSeedToSoil || collectingSoilToFert || collectingFertToWater || collectingWaterToLight || collectingLightToTemp || collectingTempToHumid || collectingHumidToLoc)
                {
                    linesTemp.Add(line);
                }
            }
        }
        collectingHumidToLoc = false;
        input.HumidToLoc = ParseMap(linesTemp.ToArray());
        linesTemp.Clear();

        return input;
    }

    private Input2 ExtractLineDataPt2(String[] lines)
    {
        var input = new Input2();
        Boolean collectingSeedToSoil = false;
        Boolean collectingSoilToFert = false;
        Boolean collectingFertToWater = false;
        Boolean collectingWaterToLight = false;
        Boolean collectingLightToTemp = false;
        Boolean collectingTempToHumid = false;
        Boolean collectingHumidToLoc = false;

        var linesTemp = new List<String>();
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (String.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
            {
                continue;
            }
            else if (line.StartsWith("seeds"))
            {
                var seedsString = line.Split("seeds:")[1];
                var seedsStrings = seedsString.Split(" ");
                var seedsIndexesAndRanges = new List<Int64>();
                foreach (var s in seedsStrings)
                {
                    var r = Int64.TryParse(s, out var sn);
                    if (r)
                        seedsIndexesAndRanges.Add(sn);
                }
                for (int k = 0; k < seedsIndexesAndRanges.Count; k += 2)
                {
                    long seedRangeStart = -1;
                    long seedRangeLength = -1;
                    if (Int64.IsEvenInteger(k))
                    {
                        seedRangeStart = seedsIndexesAndRanges[k];
                        seedRangeLength = seedsIndexesAndRanges[k + 1];
                    }
                    if (seedRangeStart != -1 && seedRangeLength != -1)
                    {
                        input.Seeds.Add((seedRangeStart, seedRangeLength));
                    }
                }
            }
            else if (line.StartsWith("seed-to-soil map:"))
            {
                collectingSeedToSoil = true;
            }
            else if (line.StartsWith("soil-to-fertilizer map:"))
            {
                input.SeedToSoil = ParseMap(linesTemp.ToArray());
                linesTemp.Clear();
                collectingSeedToSoil = false;
                collectingSeedToSoil = true;
            }
            else if (line.StartsWith("fertilizer-to-water map:"))
            {
                input.SoilToFert = ParseMap(linesTemp.ToArray());
                linesTemp.Clear();
                collectingSeedToSoil = false;
                collectingSoilToFert = true;
            }
            else if (line.StartsWith("water-to-light map:"))
            {
                input.FertToWater = ParseMap(linesTemp.ToArray());
                linesTemp.Clear();
                collectingSoilToFert = false;
                collectingWaterToLight = true;
            }
            else if (line.StartsWith("light-to-temperature map:"))
            {
                input.WaterToLight = ParseMap(linesTemp.ToArray());
                linesTemp.Clear();
                collectingWaterToLight = false;
                collectingLightToTemp = true;
            }
            else if (line.StartsWith("temperature-to-humidity map:"))
            {
                input.LightToTemp = ParseMap(linesTemp.ToArray());
                linesTemp.Clear();
                collectingLightToTemp = false;
                collectingTempToHumid = true;
            }
            else if (line.StartsWith("humidity-to-location map:"))
            {
                input.TempToHumid = ParseMap(linesTemp.ToArray());
                linesTemp.Clear();
                collectingTempToHumid = false;
                collectingHumidToLoc = true;
            }
            else
            {
                if (collectingSeedToSoil || collectingSoilToFert || collectingFertToWater || collectingWaterToLight || collectingLightToTemp || collectingTempToHumid || collectingHumidToLoc)
                {
                    linesTemp.Add(line);
                }
            }
        }
        collectingHumidToLoc = false;
        input.HumidToLoc = ParseMap(linesTemp.ToArray());
        linesTemp.Clear();

        return input;
    }


    List<SourceToDestinationInfo> ParseMap(String[] lines)
    {
        var sourceToDestinationInfos = new List<SourceToDestinationInfo>();
        foreach (var line in lines)
        {
            var sourceToDestinationInfo = new SourceToDestinationInfo();
            var numbersStrings = line.Split(" ");
            for (var i = 0; i < numbersStrings.Length; i++)
            {
                var s = numbersStrings[i];

                var r = Int64.TryParse(s, out var sn);
                if (r)
                {
                    switch (i)
                    {
                        case 0:
                            sourceToDestinationInfo.DestinationRangeStart = sn;
                            break;
                        case 1:
                            sourceToDestinationInfo.SourceRangeStart = sn;
                            break;
                        case 2:
                            sourceToDestinationInfo.RangeLength = sn;
                            break;
                        default:
                            throw new DataException("More than 3 numbers in input");
                    }
                }
            }
            sourceToDestinationInfos.Add(sourceToDestinationInfo);
        }
        return sourceToDestinationInfos;
    }

    class Input
    {
        public HashSet<Int64> Seeds { get; set; } = new();
        public List<SourceToDestinationInfo> SeedToSoil { get; set; } = new();
        public List<SourceToDestinationInfo> SoilToFert { get; set; } = new();
        public List<SourceToDestinationInfo> FertToWater { get; set; } = new();
        public List<SourceToDestinationInfo> WaterToLight { get; set; } = new();
        public List<SourceToDestinationInfo> LightToTemp { get; set; } = new();
        public List<SourceToDestinationInfo> TempToHumid { get; set; } = new();
        public List<SourceToDestinationInfo> HumidToLoc { get; set; } = new();
    }

    class Input2
    {
        public List<(Int64, Int64)> Seeds { get; set; } = new();
        public List<SourceToDestinationInfo> SeedToSoil { get; set; } = new();
        public List<SourceToDestinationInfo> SoilToFert { get; set; } = new();
        public List<SourceToDestinationInfo> FertToWater { get; set; } = new();
        public List<SourceToDestinationInfo> WaterToLight { get; set; } = new();
        public List<SourceToDestinationInfo> LightToTemp { get; set; } = new();
        public List<SourceToDestinationInfo> TempToHumid { get; set; } = new();
        public List<SourceToDestinationInfo> HumidToLoc { get; set; } = new();
    }

    class SourceToDestinationInfo
    {
        public Int64 DestinationRangeStart { get; set; }
        public Int64 SourceRangeStart { get; set; }
        public Int64 RangeLength { get; set; }

        public override String ToString()
        {
            return $"source:{SourceRangeStart}+{RangeLength}\t| dest: {DestinationRangeStart}+{RangeLength}";
        }
    }

    class SeedInfo
    {
        public Int64 SeedIndex { get; set; }
        public Int64 SoilIndex { get; set; } = -1;
        public Int64 FertIndex { get; set; } = -1;
        public Int64 WaterIndex { get; set; } = -1;
        public Int64 LightIndex { get; set; } = -1;
        public Int64 TempIndex { get; set; } = -1;
        public Int64 HumidIndex { get; set; } = -1;
        public Int64 LocIndex { get; set; } = -1;

        public override String ToString()
        {
            return $"seed: {SeedIndex}\t|so:{SoilIndex}\t|fe:{FertIndex}\t|wa:{WaterIndex}\t|li:{LightIndex}\t|te:{TempIndex}\t|hu:{HumidIndex}\t|lo:{LocIndex}";
        }
    }
}
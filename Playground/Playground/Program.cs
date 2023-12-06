using System.Diagnostics;
using Playground.aoc2023.t6;

var stopwatch = new Stopwatch();
stopwatch.Start();
Console.WriteLine($"{DateTime.Now} Playground started.");
new Task6Part2().Main();
Console.WriteLine($"{DateTime.Now} Playground finished.");
Console.WriteLine($"{DateTime.Now} Time elapsed {stopwatch.ElapsedMilliseconds} ms.");

// other
// while (true)
// {
//     ////// braces
//     //Console.WriteLine("Input braces string:");
//     //var braces = Console.ReadLine();
//     //Console.WriteLine($"This brace string is:\t{Braces.Validate(braces)}");
//
//     ////// morse code letter guesser thingy
//     ////// use ? to replace . or -
//     Console.WriteLine("Input morse code:");
//     var letterSigns = Console.ReadLine();
//     Console.WriteLine($"Possible letters: " +
//         $"{String.Join(",", MorseCode.GetPossibleMatches(letterSigns))}");
// }
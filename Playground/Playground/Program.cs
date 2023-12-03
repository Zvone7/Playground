using System.Diagnostics;
using System.Threading.Channels;
using Playground.aoc2023.t1;
using Playground.aoc2023.t2;

var stopwatch = new Stopwatch();
stopwatch.Start();
Console.WriteLine($"{DateTime.Now} Playground started.");
var task = new Task1();
// var task = new Task1_debug();
task.Main();
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
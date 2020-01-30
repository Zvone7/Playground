using System;
using System.Collections.Generic;
using System.Linq;

namespace StuffAndThings
{
    public class MorseCode
    {
        const int maxLetterLength = 3;
        public static List<String> GetPossibleMatches(String signs)
        {
            var letters = new Dictionary<char, char[]>()
            {
                { 'A', new char[]{'.','-','x' } },
                { 'E', new char[]{'.','x','x' } },
                { 'G', new char[]{'-','-','.' } },
                { 'I', new char[]{'.','.','x' } },
                { 'K', new char[]{'-','.','-' } },
                { 'M', new char[]{'-','-','x' } },
                { 'N', new char[]{'-','.','x' } },
                { 'O', new char[]{'-','-','-' } },
                { 'R', new char[]{'.','-','.' } },
                { 'S', new char[]{'.','.','.' } },
                { 'T', new char[]{'-','x','x' } },
                { 'W', new char[]{'.','-','-' } }
            };

            var result = "";
            var signsArray = signs.ToCharArray();

            var possibleLetters = letters.ToDictionary(x => x.Key, x => x.Value);
            var i = 0;

            foreach (var letter in signsArray)
            {
                if (letter == '?')
                {
                    var temp = possibleLetters
                        .Where(x => x.Value[i] != 'x' &&
                        (maxLetterLength - x.Value.Count(y => y == 'x') == signsArray.Count()));
                    if (temp.Count() > 0)
                    {
                        possibleLetters = new Dictionary<char, char[]>();
                        foreach (var l in temp)
                        {
                            possibleLetters.Add(l.Key, l.Value);
                        }
                    }
                }
                else
                {
                    var temp = possibleLetters
                        .Where(x => x.Value[i] != 'x' &&
                        x.Value[i] == letter &&
                        (maxLetterLength - x.Value.Count(y => y == 'x')) == signsArray.Count());
                    if (temp.Count() > 0)
                    {
                        possibleLetters = new Dictionary<char, char[]>();
                        foreach (var l in temp)
                        {
                            possibleLetters.Add(l.Key, l.Value);
                        }
                    }
                }
                i++;
            }

            return possibleLetters.Select(x => x.Key.ToString()).ToList();
        }
    }
}

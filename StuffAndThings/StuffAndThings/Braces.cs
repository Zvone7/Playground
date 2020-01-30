using System;
using System.Collections.Generic;

namespace StuffAndThings
{
    public class Braces
    {
        public static Boolean Validate(String braces)
        {
            var stack = new Stack<char>();

            foreach (var brace in braces.ToCharArray())
            {
                if (brace == ')')
                {
                    if (stack.Count == 0) return false;
                    var existingBrace = stack.Pop();
                    if (existingBrace != '(') return false;
                }
                else if (brace == ']')
                {
                    if (stack.Count == 0) return false;
                    var existingBrace = stack.Pop();
                    if (existingBrace != '[') return false;
                }
                else if (brace == '}')
                {
                    if (stack.Count == 0) return false;
                    var existingBrace = stack.Pop();
                    if (existingBrace != '{') return false;
                }
                else
                {
                    stack.Push(brace);
                }

            }
            return stack.Count == 0;

        }
    }
}

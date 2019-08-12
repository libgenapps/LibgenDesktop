using System;
using System.Linq;
using System.Text;

namespace LibgenDesktop.Models.Utils
{
    internal static class StringExtensions
    {
        public static bool CompareOrdinalIgnoreCase(this string currentString, string otherString)
        {
            return String.Compare(currentString, otherString, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public static string SeparateCjkCharactersWithSpaces(this string input)
        {
            if (String.IsNullOrEmpty(input) || input.All(c => !IsCjkCharacter(c)))
            {
                return input;
            }
            StringBuilder resultBuilder = new StringBuilder();
            bool previousCharacterIsCjk = false;
            foreach (char c in input)
            {
                if (IsCjkCharacter(c))
                {
                    if (previousCharacterIsCjk)
                    {
                        resultBuilder.Append(" ");
                    }
                    previousCharacterIsCjk = true;
                }
                else
                {
                    previousCharacterIsCjk = false;
                }
                resultBuilder.Append(c);
            }
            return resultBuilder.ToString();
        }

        private static bool IsCjkCharacter(char c)
        {
            int charCode = c;
            return
                (charCode >= 0x4E00 && charCode <= 0x9FFF) ||
                (charCode >= 0x3400 && charCode <= 0x4DBF) ||
                (charCode >= 0xF900 && charCode <= 0xFAFF);
        }
    }
}

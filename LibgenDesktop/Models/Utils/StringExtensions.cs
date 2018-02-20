using System;

namespace LibgenDesktop.Models.Utils
{
    internal static class StringExtensions
    {
        public static bool CompareOrdinalIgnoreCase(this string currentString, string otherString)
        {
            return String.Compare(currentString, otherString, StringComparison.OrdinalIgnoreCase) == 0;
        }
    }
}

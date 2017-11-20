using System;
using System.Globalization;
using System.Text;

namespace LibgenDesktop.Models.Utils
{
    internal static class Formatters
    {
        private static readonly NumberFormatInfo thousandsSeparatedNumberFormat;
        private static readonly string[] fileSizePostfixes;

        static Formatters()
        {
            thousandsSeparatedNumberFormat = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            thousandsSeparatedNumberFormat.NumberGroupSeparator = " ";
            fileSizePostfixes = new[] { "байт", "КБ", "МБ", "ГБ", "ТБ" };
        }

        public static NumberFormatInfo ThousandsSeparatedNumberFormat => thousandsSeparatedNumberFormat;

        public static string FileSizeToString(long fileSize, bool showBytes)
        {
            int postfixIndex = fileSize != 0 ? (int)Math.Floor(Math.Log(fileSize) / Math.Log(1024)) : 0;
            StringBuilder resultBuilder = new StringBuilder();
            resultBuilder.Append((fileSize / Math.Pow(1024, postfixIndex)).ToString("N2"));
            resultBuilder.Append(" ");
            resultBuilder.Append(fileSizePostfixes[postfixIndex]);
            if (showBytes && postfixIndex != 0)
            {
                resultBuilder.Append(" (");
                resultBuilder.Append(fileSize.ToString("N0", thousandsSeparatedNumberFormat));
                resultBuilder.Append(" байт)");
            }
            return resultBuilder.ToString();
        }
    }
}

using System.Globalization;

namespace LibgenDesktop.Interface
{
    internal static class Formatters
    {
        private static readonly NumberFormatInfo bookCountFormat;

        static Formatters()
        {
            bookCountFormat = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            bookCountFormat.NumberGroupSeparator = " ";
        }

        public static NumberFormatInfo BookCountFormat => bookCountFormat;
    }
}

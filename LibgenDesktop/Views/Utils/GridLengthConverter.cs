using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using InternalGridLengthConverter = System.Windows.GridLengthConverter;

namespace LibgenDesktop.Views.Utils
{
    internal class GridLengthConverter : IValueConverter
    {
        private readonly InternalGridLengthConverter internalGridLengthConverter;

        public GridLengthConverter()
        {
            internalGridLengthConverter = new InternalGridLengthConverter();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return internalGridLengthConverter.ConvertFrom(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is GridLength gridLength)
            {
                return gridLength.Value;
            }
            return null;
        }
    }
}

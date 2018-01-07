using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LibgenDesktop.Views.Utils
{
    internal class BooleanToWindowStateConverter : IValueConverter
    {
        public BooleanToWindowStateConverter()
        {
            TrueValue = WindowState.Maximized;
            FalseValue = WindowState.Normal;
        }

        public WindowState TrueValue { get; set; }
        public WindowState FalseValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
            {
                return null;
            }
            return (bool)value ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Equals(value, TrueValue))
            {
                return true;
            }
            if (Equals(value, FalseValue))
            {
                return false;
            }
            return null;
        }
    }
}

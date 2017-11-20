using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using InternalDataGridLengthConverter = System.Windows.Controls.DataGridLengthConverter;

namespace LibgenDesktop.Views.Utils
{
    internal class DataGridLengthConverter : IValueConverter
    {
        private readonly InternalDataGridLengthConverter internalDataGridLengthConverter;

        public DataGridLengthConverter()
        {
            internalDataGridLengthConverter = new InternalDataGridLengthConverter();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return internalDataGridLengthConverter.ConvertFrom(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DataGridLength dataGridLength)
            {
                return dataGridLength.DisplayValue;
            }
            return null;
        }
    }
}

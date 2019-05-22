using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LibgenDesktop.Views.Utils
{
    internal class DataGridExtensions
    {
        public static DependencyProperty DoubleClickCommandProperty =
            DependencyProperty.RegisterAttached("DoubleClickCommand", typeof(ICommand), typeof(DataGridExtensions),
                new PropertyMetadata(DoubleClick_PropertyChanged));

        public static void SetDoubleClickCommand(UIElement element, ICommand value)
        {
            element.SetValue(DoubleClickCommandProperty, value);
        }

        public static ICommand GetDoubleClickCommand(UIElement element)
        {
            return (ICommand)element.GetValue(DoubleClickCommandProperty);
        }

        private static void DoubleClick_PropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (!(dependencyObject is DataGridRow row))
            {
                return;
            }
            if (e.NewValue != null)
            {
                row.AddHandler(Control.MouseDoubleClickEvent, new RoutedEventHandler(DataGrid_MouseDoubleClick));
            }
            else
            {
                row.RemoveHandler(Control.MouseDoubleClickEvent, new RoutedEventHandler(DataGrid_MouseDoubleClick));
            }
        }

        private static void DataGrid_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (sender is DataGridRow row)
            {
                ICommand command = GetDoubleClickCommand(row);
                if (command.CanExecute(row.Item))
                {
                    command.Execute(row.Item);
                }
            }
        }
    }
}

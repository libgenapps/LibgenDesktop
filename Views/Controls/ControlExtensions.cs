using System.Windows;
using System.Windows.Controls;

namespace LibgenDesktop.Views.Controls
{
    internal static class ControlExtensions
    {
        public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.RegisterAttached("MaxLength", typeof(int), typeof(ControlExtensions), new UIPropertyMetadata(OnMaxLengthChanged));

        public static int GetMaxLength(DependencyObject dependencyObject)
        {
            return (int)dependencyObject.GetValue(MaxLengthProperty);
        }

        public static void SetMaxLength(DependencyObject dependencyObject, int value)
        {
            dependencyObject.SetValue(MaxLengthProperty, value);
        }

        private static void OnMaxLengthChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is ComboBox comboBox)
            {
                comboBox.Loaded += (sender, args) =>
                {
                    if (comboBox.Template.FindName("PART_EditableTextBox", comboBox) is TextBox textBox)
                    {
                        textBox.SetValue(TextBox.MaxLengthProperty, e.NewValue);
                    }
                };
            }
        }
    }
}

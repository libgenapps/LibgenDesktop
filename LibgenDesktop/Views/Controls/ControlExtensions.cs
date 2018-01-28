using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

        public static T FindChild<T>(this DependencyObject parent, string childName = null) where T : DependencyObject
        {
            if (parent == null)
            {
                return null;
            }
            T result = null;
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                T childType = child as T;
                if (childType == null)
                {
                    result = FindChild<T>(child, childName);
                    if (result != null)
                    {
                        break;
                    }
                }
                else if (!String.IsNullOrEmpty(childName))
                {
                    if (child is FrameworkElement frameworkElement && frameworkElement.Name == childName)
                    {
                        result = (T)child;
                        break;
                    }
                }
                else
                {
                    result = (T)child;
                    break;
                }
            }
            return result;
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

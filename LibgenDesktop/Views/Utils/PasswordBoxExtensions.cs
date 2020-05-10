using System.Windows;
using System.Windows.Controls;

namespace LibgenDesktop.Views.Utils
{
    internal class PasswordBoxExtensions : DependencyObject
    {
        public static readonly DependencyProperty PasswordBindingProperty = DependencyProperty.RegisterAttached("PasswordBinding", typeof(string),
            typeof(PasswordBoxExtensions), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPasswordBindingChanged));

        private static bool isUpdating;

        public static string GetPasswordBinding(DependencyObject dependencyObject)
        {
            return (string)dependencyObject.GetValue(PasswordBindingProperty);
        }

        public static void SetPasswordBinding(DependencyObject dependencyObject, string value)
        {
            dependencyObject.SetValue(PasswordBindingProperty, value);
        }

        private static void OnPasswordBindingChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is PasswordBox passwordBox)
            {
                passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
                if (e.Property == PasswordBindingProperty)
                {
                    if (!isUpdating)
                    {
                        isUpdating = true;
                        passwordBox.Password = e.NewValue as string;
                        isUpdating = false;
                    }
                }
            }
        }

        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                isUpdating = true;
                SetPasswordBinding(passwordBox, passwordBox.Password);
                isUpdating = false;
            }
        }
    }
}

using System;
using System.Windows;
using System.Windows.Controls;

namespace LibgenDesktop.Views.Utils
{
    internal class PasswordBoxExtensions : DependencyObject
    {
        public static readonly DependencyProperty PasswordBindingProperty = DependencyProperty.RegisterAttached("PasswordBinding", typeof(string),
            typeof(PasswordBoxExtensions), new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPasswordBindingChanged));

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
            PasswordBox passwordBox = dependencyObject as PasswordBox;
            if (passwordBox == null)
            {
                return;
            }
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

        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            if (passwordBox == null)
            {
                return;
            }
            isUpdating = true;
            SetPasswordBinding(passwordBox, passwordBox.Password);
            isUpdating = false;
        }
    }
}

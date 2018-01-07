using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using LibgenDesktop.Infrastructure;

namespace LibgenDesktop.Views.Controls
{
    public partial class BookAttributeValueLabel
    {
        public BookAttributeValueLabel()
        {
            InitializeComponent();
            DependencyPropertyDescriptor propertyDescriptor = DependencyPropertyDescriptor.FromProperty(TextProperty, typeof(TextBlock));
            propertyDescriptor.AddValueChanged(this, TextChanged);
        }

        private void CopyMenuItemClick(object sender, RoutedEventArgs e)
        {
            WindowManager.SetClipboardText(Text);
        }

        private void TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(Text))
            {
                ContextMenu labelContextMenu = Resources["labelContextMenu"] as ContextMenu;
                MenuItem copyMenuItem = labelContextMenu.Items[0] as MenuItem;
                copyMenuItem.Header = $"Копировать \"{Text}\"";
                ContextMenu = labelContextMenu;
            }
        }
    }
}

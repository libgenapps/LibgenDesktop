using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using LibgenDesktop.Infrastructure;

namespace LibgenDesktop.Views.Controls
{
    public partial class BookAttributeValueLabel
    {
        public static readonly DependencyProperty ContextMenuItemFormatProperty = DependencyProperty.Register("ContextMenuItemFormat", typeof(string),
            typeof(BookAttributeValueLabel), new PropertyMetadata(OnContextMenuItemFormatChanged));

        public BookAttributeValueLabel()
        {
            InitializeComponent();
            DependencyPropertyDescriptor propertyDescriptor = DependencyPropertyDescriptor.FromProperty(TextProperty, typeof(TextBlock));
            propertyDescriptor.AddValueChanged(this, TextChanged);
        }

        public string ContextMenuItemFormat
        {
            get
            {
                return (string)GetValue(ContextMenuItemFormatProperty);
            }
            set
            {
                SetValue(ContextMenuItemFormatProperty, value);
            }
        }

        private void CopyMenuItemClick(object sender, RoutedEventArgs e)
        {
            WindowManager.SetClipboardText(Text);
        }

        private void CreateContextMenu()
        {
            if (!String.IsNullOrWhiteSpace(Text))
            {
                ContextMenu labelContextMenu = Resources["labelContextMenu"] as ContextMenu;
                MenuItem copyMenuItem = labelContextMenu.Items[0] as MenuItem;
                string contextMenuItemFormat = ContextMenuItemFormat;
                if (contextMenuItemFormat.Contains("{0}"))
                {
                    copyMenuItem.Header = String.Format(contextMenuItemFormat, Text);
                }
                else
                {
                    copyMenuItem.Header = contextMenuItemFormat;
                }
                ContextMenu = labelContextMenu;
            }
        }

        private void TextChanged(object sender, EventArgs e)
        {
            CreateContextMenu();
        }

        private static void OnContextMenuItemFormatChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is BookAttributeValueLabel bookAttributeValueLabel)
            {
                bookAttributeValueLabel.CreateContextMenu();
            }
        }
    }
}

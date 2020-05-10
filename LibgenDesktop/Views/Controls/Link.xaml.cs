using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LibgenDesktop.Infrastructure;

namespace LibgenDesktop.Views.Controls
{
    public partial class Link
    {
        public static readonly DependencyProperty UrlProperty = DependencyProperty.Register("Url", typeof(string), typeof(Link));

        public static readonly DependencyProperty CopyContextMenuItemTextProperty = DependencyProperty.Register("CopyContextMenuItemText", typeof(string),
            typeof(Link), new PropertyMetadata(OnCopyContextMenuItemTextChanged));

        public Link()
        {
            InitializeComponent();
        }

        public string Url
        {
            get
            {
                return (string)GetValue(UrlProperty);
            }
            set
            {
                SetValue(UrlProperty, value);
            }
        }

        public string CopyContextMenuItemText
        {
            get
            {
                return (string)GetValue(CopyContextMenuItemTextProperty);
            }
            set
            {
                SetValue(CopyContextMenuItemTextProperty, value);
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (!String.IsNullOrWhiteSpace(Url))
            {
                Process.Start(Url);
            }
        }

        private void CopyMenuItemClick(object sender, RoutedEventArgs e)
        {
            WindowManager.SetClipboardText(Url);
        }

        private void CreateContextMenu()
        {
            ContextMenu linkContextMenu = Resources["linkContextMenu"] as ContextMenu;
            MenuItem copyMenuItem = linkContextMenu.Items[0] as MenuItem;
            copyMenuItem.Header = CopyContextMenuItemText;
            ContextMenu = linkContextMenu;
        }

        private static void OnCopyContextMenuItemTextChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is Link link)
            {
                link.CreateContextMenu();
            }
        }
    }
}

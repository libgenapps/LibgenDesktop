using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace LibgenDesktop.Views.Controls
{
    internal class DownloaderListBox : ListBox
    {
        public static readonly DependencyProperty SelectionChangedCommandProperty = DependencyProperty.Register("SelectionChangedCommand", typeof(ICommand), typeof(DownloaderListBox));
        public static readonly DependencyProperty DoubleClickCommandProperty = DependencyProperty.Register("DoubleClickCommand", typeof(ICommand), typeof(DownloaderListBox));

        public ICommand SelectionChangedCommand
        {
            get
            {
                return (ICommand)GetValue(SelectionChangedCommandProperty);
            }
            set
            {
                SetValue(SelectionChangedCommandProperty, value);
            }
        }

        public ICommand DoubleClickCommand
        {
            get
            {
                return (ICommand)GetValue(DoubleClickCommandProperty);
            }
            set
            {
                SetValue(DoubleClickCommandProperty, value);
            }
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            ICommand selectionChangedCommand = SelectionChangedCommand;
            if (selectionChangedCommand != null)
            {
                selectionChangedCommand.Execute(null);
            }
            base.OnSelectionChanged(e);
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            ICommand doubleClickCommand = DoubleClickCommand;
            if (doubleClickCommand != null)
            {
                doubleClickCommand.Execute(null);
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            HitTestResult r = VisualTreeHelper.HitTest(this, e.GetPosition(this));
            if (r.VisualHit.GetType() != typeof(ListBoxItem))
            {
                UnselectAll();
            }
        }
    }
}

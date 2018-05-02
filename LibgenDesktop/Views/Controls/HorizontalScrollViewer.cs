using System.Windows.Controls;
using System.Windows.Input;

namespace LibgenDesktop.Views.Controls
{
    internal class HorizontalScrollViewer : ScrollViewer
    {
        public HorizontalScrollViewer()
        {
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
        }

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                LineLeft();
                LineLeft();
                LineLeft();
            }
            else
            {
                LineRight();
                LineRight();
                LineRight();
            }
            e.Handled = true;
        }
    }
}

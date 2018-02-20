using System.Windows.Controls;

namespace LibgenDesktop.Views.Controls
{
    internal class AutoScrollViewer : ScrollViewer
    {
        private bool autoScroll;

        public AutoScrollViewer()
        {
            autoScroll = false;
        }

        protected override void OnScrollChanged(ScrollChangedEventArgs e)
        {
            base.OnScrollChanged(e);
            if (e.ExtentHeightChange == 0)
            {
                autoScroll = VerticalOffset == ScrollableHeight;
            }
            if (autoScroll && e.ExtentHeightChange != 0)
            {
                ScrollToVerticalOffset(ExtentHeight);
            }
        }
    }
}

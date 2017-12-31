using System.Windows;
using System.Windows.Input;
using Dragablz;

namespace LibgenDesktop.Views.Controls
{
    public class TabControl : TabablzControl
    {
        public static readonly DependencyProperty CloseTabCommandProperty = DependencyProperty.Register("CloseTabCommand", typeof(ICommand), typeof(TabControl));

        public ICommand CloseTabCommand
        {
            get
            {
                return (ICommand)GetValue(CloseTabCommandProperty);
            }
            set
            {
                SetValue(CloseTabCommandProperty, value);
            }
        }
    }
}

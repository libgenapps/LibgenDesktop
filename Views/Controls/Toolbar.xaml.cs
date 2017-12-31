using System.Windows;
using System.Windows.Media;

namespace LibgenDesktop.Views.Controls
{
    public partial class Toolbar
    {
        public static readonly DependencyProperty IconForegroundProperty = DependencyProperty.Register("IconForeground", typeof(Brush), typeof(Toolbar));

        public Toolbar()
        {
            InitializeComponent();
        }

        public Brush IconForeground
        {
            get
            {
                return (Brush)GetValue(IconForegroundProperty);
            }
            set
            {
                SetValue(IconForegroundProperty, value);
            }
        }
    }
}

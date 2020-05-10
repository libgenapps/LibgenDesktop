using System.Windows;
using static LibgenDesktop.Common.Constants;

namespace LibgenDesktop.Views.Controls
{
    public partial class LanguageComboBox
    {
        public static readonly DependencyProperty PercentTranslatedFontSizeProperty =
            DependencyProperty.Register("PercentTranslatedFontSize", typeof(double), typeof(LanguageComboBox), new PropertyMetadata(DEFAULT_FONT_SIZE));

        public LanguageComboBox()
        {
            InitializeComponent();
        }

        public double PercentTranslatedFontSize
        {
            get
            {
                return (double)GetValue(PercentTranslatedFontSizeProperty);
            }
            set
            {
                SetValue(PercentTranslatedFontSizeProperty, value);
            }
        }
    }
}

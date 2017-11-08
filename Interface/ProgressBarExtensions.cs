using System.Windows.Forms;

namespace LibgenDesktop.Interface
{
    internal static class ProgressBarExtensions
    {
        public static void SetProgressNoAnimation(this ProgressBar progressBar, int value)
        {
            // To get around the progressive animation, we need to move the progress bar backwards.
            if (value == progressBar.Maximum)
            {
                progressBar.Maximum = value + 1;
                progressBar.Value = value + 1;
                progressBar.Maximum = value;
            }
            else
            {
                progressBar.Value = value + 1;
                progressBar.Value = value;
            }
        }
    }
}

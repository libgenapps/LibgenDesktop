using System;
using System.Windows.Forms;

namespace LibgenDesktop.Interface
{
    public partial class ErrorForm : Form
    {
        public ErrorForm(string errorDetails)
        {
            InitializeComponent();
            detailsTextBox.Text = errorDetails;
        }

        private void copyButton_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(detailsTextBox.Text);
        }

        private void detailsTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && (e.KeyCode == Keys.A))
            {
                if (sender != null)
                {
                    ((TextBox)sender).SelectAll();
                }
                e.Handled = true;
            }
        }
    }
}

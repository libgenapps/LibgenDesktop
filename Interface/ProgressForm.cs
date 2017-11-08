using System;
using System.Windows.Forms;
using LibgenDesktop.Infrastructure;

namespace LibgenDesktop.Interface
{
    internal partial class ProgressForm : Form
    {
        private ProgressOperation progressOperation;
        private DateTime operationStartTime;
        private TimeSpan displayingElapsedTime;

        public ProgressForm(ProgressOperation progressOperation)
        {
            InitializeComponent();
            this.progressOperation = progressOperation;
            Text = progressOperation.Title;
            progressOperation.ProgressEvent += ProgressOperation_ProgressEvent;
            progressOperation.CompletedEvent += ProgressOperation_CompletedEvent;
            progressOperation.CancelledEvent += ProgressOperation_CancelledEvent;
        }

        private void ProgressForm_Shown(object sender, EventArgs e)
        {
            operationStartTime = DateTime.Now;
            displayingElapsedTime = TimeSpan.Zero;
            estimatedTimeLabel.Text = "Прошло 00:00";
            progressOperation.Start();
        }

        private void ProgressOperation_ProgressEvent(object sender, ProgressEventArgs e)
        {
            BeginInvoke(new Action(() =>
            {
                DateTime now = DateTime.Now;
                TimeSpan elapsed = now - operationStartTime;
                if (elapsed.Seconds != displayingElapsedTime.Seconds)
                {
                    estimatedTimeLabel.Text = $"Прошло {elapsed:mm\\:ss}";
                    displayingElapsedTime = elapsed;
                    if (e.PercentCompleted > 5)
                    {
                        TimeSpan remaining = TimeSpan.FromSeconds(elapsed.TotalSeconds / e.PercentCompleted * (100 - e.PercentCompleted));
                        estimatedTimeLabel.Text += $", осталось {remaining:mm\\:ss}";
                    }
                }
                progressDescription.Text = e.ProgressDescription;
                progressBar.SetProgressNoAnimation((int)Math.Truncate(e.PercentCompleted * 10));
            }));
        }

        private void ProgressOperation_CompletedEvent(object sender, EventArgs e)
        {
            BeginInvoke(new Action(() => Close()));
        }

        private void ProgressOperation_CancelledEvent(object sender, EventArgs e)
        {
            BeginInvoke(new Action(() => Close()));
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ProgressForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            progressOperation?.Cancel();
            RemoveOperation();
        }

        private void RemoveOperation()
        {
            ProgressOperation removingProgressOperation = progressOperation;
            if (removingProgressOperation != null)
            {
                removingProgressOperation.ProgressEvent -= ProgressOperation_ProgressEvent;
                removingProgressOperation.CompletedEvent -= ProgressOperation_CompletedEvent;
                removingProgressOperation.CancelledEvent -= ProgressOperation_CancelledEvent;
                progressOperation = null;
            }
        }
    }
}

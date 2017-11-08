using System;

namespace LibgenDesktop.Infrastructure
{
    public class ProgressEventArgs : EventArgs
    {
        public string ProgressDescription { get; set; }
        public double PercentCompleted { get; set; }
    }
}
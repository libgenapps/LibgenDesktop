using System;

namespace LibgenDesktop.Infrastructure
{
    public class ErrorEventArgs : EventArgs
    {
        public Exception Exception { get; set; }
    }
}
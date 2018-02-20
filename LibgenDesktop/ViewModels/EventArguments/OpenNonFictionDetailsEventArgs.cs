using System;
using LibgenDesktop.Models.Entities;

namespace LibgenDesktop.ViewModels.EventArguments
{
    internal class OpenNonFictionDetailsEventArgs : EventArgs
    {
        public OpenNonFictionDetailsEventArgs(NonFictionBook nonFictionBook)
        {
            NonFictionBook = nonFictionBook;
        }

        public NonFictionBook NonFictionBook { get; }
    }
}

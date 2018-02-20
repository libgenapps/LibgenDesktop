using System;
using LibgenDesktop.Models.Entities;

namespace LibgenDesktop.ViewModels.EventArguments
{
    internal class OpenFictionDetailsEventArgs : EventArgs
    {
        public OpenFictionDetailsEventArgs(FictionBook fictionBook)
        {
            FictionBook = fictionBook;
        }

        public FictionBook FictionBook { get; }
    }
}

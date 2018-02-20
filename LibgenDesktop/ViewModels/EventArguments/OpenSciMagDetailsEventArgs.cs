using System;
using LibgenDesktop.Models.Entities;

namespace LibgenDesktop.ViewModels.EventArguments
{
    internal class OpenSciMagDetailsEventArgs : EventArgs
    {
        public OpenSciMagDetailsEventArgs(SciMagArticle sciMagArticle)
        {
            SciMagArticle = sciMagArticle;
        }

        public SciMagArticle SciMagArticle { get; }
    }
}

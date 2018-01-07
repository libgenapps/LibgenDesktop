using System.Collections.Generic;

namespace LibgenDesktop.Infrastructure
{
    internal class OpenFileDialogResult
    {
        public bool DialogResult { get; set; }
        public List<string> SelectedFilePaths { get; set; }
    }
}
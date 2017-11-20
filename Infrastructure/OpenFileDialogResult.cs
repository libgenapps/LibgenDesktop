using System.Collections.Generic;

namespace LibgenDesktop.Infrastructure
{
    public class OpenFileDialogResult
    {
        public bool DialogResult { get; set; }
        public List<string> SelectedFilePaths { get; set; }
    }
}
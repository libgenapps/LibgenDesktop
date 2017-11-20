namespace LibgenDesktop.Infrastructure
{
    internal class OpenFileDialogParameters
    {
        public string DialogTitle { get; set; }
        public string Filter { get; set; }
        public bool Multiselect { get; set; }
        public string InitialDirectory { get; set; }
    }
}
namespace LibgenDesktop.Infrastructure
{
    internal class SaveFileDialogParameters
    {
        public string DialogTitle { get; set; }
        public string Filter { get; set; }
        public bool OverwritePrompt { get; set; }
        public string InitialDirectory { get; set; }
        public string InitialFileName { get; set; }
    }
}

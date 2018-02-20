namespace LibgenDesktop.Setup
{
    internal class AppFile
    {
        public AppFile(string sourceFilePath, string targetFilePath)
        {
            SourceFilePath = sourceFilePath;
            TargetFilePath = targetFilePath;
        }

        public string SourceFilePath { get; }
        public string TargetFilePath { get; }
    }
}

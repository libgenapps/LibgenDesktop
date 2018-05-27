using LibgenDesktop.Models.Entities;

namespace LibgenDesktop.Models.ProgressArgs
{
    internal class ScanProgress<T> where T: LibgenObject
    {
        public ScanProgress(string relativeFilePath, T libgenObject)
        {
            RelativeFilePath = relativeFilePath;
            LibgenObject = libgenObject;
        }

        public string RelativeFilePath { get; }
        public T LibgenObject { get; }
    }
}

using LibgenDesktop.Models.Entities;

namespace LibgenDesktop.ViewModels.Library
{
    internal abstract class ScanResultItemViewModel : ViewModel
    {
        public ScanResultItemViewModel(string relativeFilePath)
        {
            RelativeFilePath = relativeFilePath;
        }

        public string RelativeFilePath { get; }

        public abstract string Authors { get; }
        public abstract string Title { get; }
    }

    internal abstract class ScanResultItemViewModel<T> : ScanResultItemViewModel where T : LibgenObject
    {
        public ScanResultItemViewModel(string relativeFilePath, T libgenObject)   
            : base(relativeFilePath)
        {
            LibgenObject = libgenObject;
        }

        public T LibgenObject { get; }
    }
}

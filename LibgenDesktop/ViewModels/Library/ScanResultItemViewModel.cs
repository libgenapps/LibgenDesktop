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
        public abstract bool ExistsInLibrary { get; }
        public abstract LibgenObjectType LibgenObjectType { get; }
        public abstract int ObjectId { get; }
    }

    internal abstract class ScanResultItemViewModel<T> : ScanResultItemViewModel where T : LibgenObject
    {
        public ScanResultItemViewModel(string relativeFilePath, T libgenObject)   
            : base(relativeFilePath)
        {
            LibgenObject = libgenObject;
        }

        public T LibgenObject { get; }

        public override bool ExistsInLibrary => LibgenObject.FileId.HasValue;

        public override LibgenObjectType LibgenObjectType => LibgenObject.LibgenObjectType;

        public override int ObjectId => LibgenObject.Id;
    }
}

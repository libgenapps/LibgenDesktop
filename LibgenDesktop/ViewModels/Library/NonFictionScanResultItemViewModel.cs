using LibgenDesktop.Models.Entities;

namespace LibgenDesktop.ViewModels.Library
{
    internal class FictionScanResultItemViewModel : ScanResultItemViewModel<FictionBook>
    {
        public FictionScanResultItemViewModel(string relativeFilePath, FictionBook book)
            : base(relativeFilePath, book)
        {
        }

        public override string Authors => LibgenObject.Authors;
        public override string Title => LibgenObject.Title;
    }
}

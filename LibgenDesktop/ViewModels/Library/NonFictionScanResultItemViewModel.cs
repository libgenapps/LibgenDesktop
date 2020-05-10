using LibgenDesktop.Models.Entities;

namespace LibgenDesktop.ViewModels.Library
{
    internal class NonFictionScanResultItemViewModel : ScanResultItemViewModel<NonFictionBook>
    {
        public NonFictionScanResultItemViewModel(string relativeFilePath, NonFictionBook book)
            : base(relativeFilePath, book)
        {
        }

        public override string Authors => LibgenObject.Authors;
        public override string Title => LibgenObject.Title;
    }
}

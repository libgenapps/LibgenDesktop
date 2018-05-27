using LibgenDesktop.Models.Entities;

namespace LibgenDesktop.ViewModels.Library
{
    internal class SciMagScanResultItemViewModel : ScanResultItemViewModel<SciMagArticle>
    {
        public SciMagScanResultItemViewModel(string relativeFilePath, SciMagArticle article)
            : base(relativeFilePath, article)
        {
        }

        public override string Authors => LibgenObject.Authors;
        public override string Title => LibgenObject.Title;
    }
}

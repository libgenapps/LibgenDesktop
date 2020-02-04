using LibgenDesktop.Models.ProgressArgs;

namespace LibgenDesktop.ViewModels.Library
{
    internal class ScanResultErrorItemViewModel : ViewModel
    {
        public ScanResultErrorItemViewModel(string relativeFilePath, ErrorTypes errorTypes)
        {
            RelativeFilePath = relativeFilePath;
            ErrorType = errorTypes;
        }

        public ErrorTypes ErrorType { get; }
        public string RelativeFilePath { get; }
        public string Description { get; set; }
    }
}
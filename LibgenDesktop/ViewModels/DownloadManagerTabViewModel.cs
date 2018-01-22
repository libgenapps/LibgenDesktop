using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;

namespace LibgenDesktop.ViewModels
{
    internal class DownloadManagerTabViewModel : TabViewModel
    {
        public DownloadManagerTabViewModel(MainModel mainModel, IWindowContext parentWindowContext)
            : base(mainModel, parentWindowContext, "Загрузки")
        {
        }
    }
}

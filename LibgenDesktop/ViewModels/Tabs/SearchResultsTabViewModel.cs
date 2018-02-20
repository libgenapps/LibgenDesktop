using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;

namespace LibgenDesktop.ViewModels.Tabs
{
    internal abstract class SearchResultsTabViewModel : TabViewModel
    {
        protected SearchResultsTabViewModel(MainModel mainModel, IWindowContext parentWindowContext, string title)
            : base(mainModel, parentWindowContext, title)
        {
        }

        public abstract void ShowExportPanel();
    }
}

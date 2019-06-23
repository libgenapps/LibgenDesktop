namespace LibgenDesktop.ViewModels.Tabs
{
    internal interface ISearchResultsTabViewModel : ITabViewModel
    {
        void Search(string searchQuery);
        void ToggleExportPanel();
    }
}

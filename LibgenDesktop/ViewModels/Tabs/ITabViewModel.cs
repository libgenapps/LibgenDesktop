namespace LibgenDesktop.ViewModels.Tabs
{
    internal interface ITabViewModel
    {
        string Title { get; set; }

        void HandleTabClosing();
    }
}
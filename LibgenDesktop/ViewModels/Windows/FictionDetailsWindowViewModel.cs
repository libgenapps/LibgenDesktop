using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Settings;
using LibgenDesktop.ViewModels.Tabs;

namespace LibgenDesktop.ViewModels.Windows
{
    internal class FictionDetailsWindowViewModel : DetailsWindowViewModel<FictionBook>
    {
        public FictionDetailsWindowViewModel(MainModel mainModel, FictionBook book, bool modalWindow)
            : base(mainModel, book, modalWindow)
        {
            WindowTitle = book.Title;
            WindowWidth = mainModel.AppSettings.Fiction.DetailsWindow.Width;
            WindowHeight = mainModel.AppSettings.Fiction.DetailsWindow.Height;
        }

        protected override DetailsTabViewModel<FictionBook> CreateDetailsTabViewModel(MainModel mainModel, IWindowContext currentWindowContext,
            FictionBook libgenObject, bool modalWindow)
        {
            return new FictionDetailsTabViewModel(mainModel, CurrentWindowContext, libgenObject, modalWindow);
        }

        protected override void OnWindowClosing()
        {
            MainModel.AppSettings.Fiction.DetailsWindow = new AppSettings.FictionDetailsWindowSettings
            {
                Width = WindowWidth,
                Height = WindowHeight
            };
            MainModel.SaveSettings();
        }
    }
}

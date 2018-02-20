using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Settings;
using LibgenDesktop.ViewModels.Tabs;

namespace LibgenDesktop.ViewModels.Windows
{
    internal class NonFictionDetailsWindowViewModel : DetailsWindowViewModel<NonFictionBook>
    {
        public NonFictionDetailsWindowViewModel(MainModel mainModel, NonFictionBook book, bool modalWindow)
            : base(mainModel, book, modalWindow)
        {
            WindowTitle = book.Title;
            WindowWidth = mainModel.AppSettings.NonFiction.DetailsWindow.Width;
            WindowHeight = mainModel.AppSettings.NonFiction.DetailsWindow.Height;
        }

        protected override DetailsTabViewModel<NonFictionBook> CreateDetailsTabViewModel(MainModel mainModel, IWindowContext currentWindowContext,
            NonFictionBook libgenObject, bool modalWindow)
        {
            return new NonFictionDetailsTabViewModel(mainModel, currentWindowContext, libgenObject, modalWindow);
        }

        protected override void OnWindowClosing()
        {
            MainModel.AppSettings.NonFiction.DetailsWindow = new AppSettings.NonFictionDetailsWindowSettings
            {
                Width = WindowWidth,
                Height = WindowHeight
            };
            MainModel.SaveSettings();
        }
    }
}

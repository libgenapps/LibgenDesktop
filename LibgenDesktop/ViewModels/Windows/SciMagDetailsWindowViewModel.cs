using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Settings;
using LibgenDesktop.ViewModels.Tabs;

namespace LibgenDesktop.ViewModels.Windows
{
    internal class SciMagDetailsWindowViewModel : DetailsWindowViewModel<SciMagArticle>
    {
        public SciMagDetailsWindowViewModel(MainModel mainModel, SciMagArticle article, bool modalWindow)
            : base(mainModel, article, modalWindow)
        {
            WindowTitle = article.Title;
            WindowWidth = mainModel.AppSettings.SciMag.DetailsWindow.Width;
            WindowHeight = mainModel.AppSettings.SciMag.DetailsWindow.Height;
        }

        protected override DetailsTabViewModel<SciMagArticle> CreateDetailsTabViewModel(MainModel mainModel, IWindowContext currentWindowContext,
            SciMagArticle libgenObject, bool modalWindow)
        {
            return new SciMagDetailsTabViewModel(mainModel, CurrentWindowContext, libgenObject, modalWindow);
        }

        protected override void OnWindowClosing()
        {
            MainModel.AppSettings.SciMag.DetailsWindow = new AppSettings.SciMagDetailsWindowSettings
            {
                Width = WindowWidth,
                Height = WindowHeight
            };
            MainModel.SaveSettings();
        }
    }
}

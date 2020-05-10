using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Localization;
using LibgenDesktop.Models.Localization.Localizators.Tabs;
using LibgenDesktop.Models.Settings;
using LibgenDesktop.Models.Utils;
using LibgenDesktop.ViewModels.DetailsItems;

namespace LibgenDesktop.ViewModels.Tabs
{
    internal class SciMagDetailsTabViewModel : DetailsTabViewModel<SciMagArticle>
    {
        private SciMagDetailsTabLocalizator localization;
        private SciMagDetailsItemViewModel detailsItem;

        public SciMagDetailsTabViewModel(MainModel mainModel, IWindowContext parentWindowContext, SciMagArticle article, bool isInModalWindow)
            : base(mainModel, parentWindowContext, article, article.Title, isInModalWindow, mainModel.AppSettings.Mirrors.ArticlesMirrorName, null)
        {
            localization = mainModel.Localization.CurrentLanguage.SciMagDetailsTab;
        }

        public SciMagDetailsTabLocalizator Localization
        {
            get
            {
                return localization;
            }
            set
            {
                localization = value;
                NotifyPropertyChanged();
            }
        }

        public SciMagDetailsItemViewModel DetailsItem
        {
            get
            {
                if (detailsItem == null)
                {
                    detailsItem = new SciMagDetailsItemViewModel(LibgenObject, MainModel.Localization.CurrentLanguage);
                }
                return detailsItem;
            }
        }

        protected override string FileNameWithoutExtension => $"{DetailsItem.Authors} - {DetailsItem.Title}";
        protected override string FileExtension => "pdf";
        protected override string Md5Hash => DetailsItem.Md5Hash;
        protected override bool HasCover => false;

        protected override string GenerateDownloadUrl(Mirrors.MirrorConfiguration mirrorConfiguration)
        {
            return UrlGenerator.GetSciMagDownloadUrl(mirrorConfiguration, DetailsItem.Article);
        }

        protected override string GenerateCoverUrl(Mirrors.MirrorConfiguration mirrorConfiguration)
        {
            return null;
        }

        protected override string GetDownloadTransformations(Mirrors.MirrorConfiguration mirrorConfiguration)
        {
            return mirrorConfiguration.SciMagDownloadTransformations;
        }

        protected override void UpdateLocalization(Language newLanguage)
        {
            Localization = MainModel.Localization.CurrentLanguage.SciMagDetailsTab;
            DetailsItem.UpdateLocalization(newLanguage);
        }
    }
}

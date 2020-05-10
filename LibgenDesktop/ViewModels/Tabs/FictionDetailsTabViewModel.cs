using System;
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
    internal class FictionDetailsTabViewModel : DetailsTabViewModel<FictionBook>
    {
        private FictionDetailsTabLocalizator localization;
        private FictionDetailsItemViewModel detailsItem;

        public FictionDetailsTabViewModel(MainModel mainModel, IWindowContext parentWindowContext, FictionBook book, bool isInModalWindow)
            : base(mainModel, parentWindowContext, book, book.Title, isInModalWindow, mainModel.AppSettings.Mirrors.FictionBooksMirrorName,
                  mainModel.AppSettings.Mirrors.FictionCoversMirrorName)
        {
            localization = mainModel.Localization.CurrentLanguage.FictionDetailsTab;
        }

        public FictionDetailsTabLocalizator Localization
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

        public FictionDetailsItemViewModel DetailsItem
        {
            get
            {
                if (detailsItem == null)
                {
                    detailsItem = new FictionDetailsItemViewModel(LibgenObject, MainModel.Localization.CurrentLanguage);
                }
                return detailsItem;
            }
        }

        protected override string FileNameWithoutExtension => $"{DetailsItem.Authors} - {DetailsItem.Title}";
        protected override string FileExtension => DetailsItem.Format;
        protected override string Md5Hash => DetailsItem.Md5Hash;
        protected override bool HasCover => !String.IsNullOrWhiteSpace(DetailsItem.Book.CoverUrl);

        protected override string GenerateDownloadUrl(Mirrors.MirrorConfiguration mirrorConfiguration)
        {
            return UrlGenerator.GetFictionDownloadUrl(mirrorConfiguration, DetailsItem.Book);
        }

        protected override string GenerateCoverUrl(Mirrors.MirrorConfiguration mirrorConfiguration)
        {
            return UrlGenerator.GetFictionCoverUrl(mirrorConfiguration, DetailsItem.Book);
        }

        protected override string GetDownloadTransformations(Mirrors.MirrorConfiguration mirrorConfiguration)
        {
            return mirrorConfiguration.FictionDownloadTransformations;
        }

        protected override void UpdateLocalization(Language newLanguage)
        {
            Localization = MainModel.Localization.CurrentLanguage.FictionDetailsTab;
            DetailsItem.UpdateLocalization(newLanguage);
        }
    }
}

using System;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Localization;
using LibgenDesktop.Models.Localization.Localizators;
using LibgenDesktop.Models.Settings;
using LibgenDesktop.Models.Utils;
using LibgenDesktop.ViewModels.DetailsItems;

namespace LibgenDesktop.ViewModels.Tabs
{
    internal class NonFictionDetailsTabViewModel : DetailsTabViewModel<NonFictionBook>
    {
        private NonFictionDetailsTabLocalizator localization;
        private NonFictionDetailsItemViewModel detailsItem;

        public NonFictionDetailsTabViewModel(MainModel mainModel, IWindowContext parentWindowContext, NonFictionBook book, bool isInModalWindow)
            : base(mainModel, parentWindowContext, book, book.Title, isInModalWindow, mainModel.AppSettings.Mirrors.NonFictionBooksMirrorName,
                  mainModel.AppSettings.Mirrors.NonFictionCoversMirrorName)
        {
            localization = mainModel.Localization.CurrentLanguage.NonFictionDetailsTab;
        }

        public NonFictionDetailsTabLocalizator Localization
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

        public NonFictionDetailsItemViewModel DetailsItem
        {
            get
            {
                if (detailsItem == null)
                {
                    detailsItem = new NonFictionDetailsItemViewModel(LibgenObject, MainModel.Localization.CurrentLanguage);
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
            return UrlGenerator.GetNonFictionDownloadUrl(mirrorConfiguration, DetailsItem.Book);
        }

        protected override string GenerateCoverUrl(Mirrors.MirrorConfiguration mirrorConfiguration)
        {
            return UrlGenerator.GetNonFictionCoverUrl(mirrorConfiguration, DetailsItem.Book);
        }

        protected override string GetDownloadTransformations(Mirrors.MirrorConfiguration mirrorConfiguration)
        {
            return mirrorConfiguration.NonFictionDownloadTransformations;
        }

        protected override void UpdateLocalization(Language newLanguage)
        {
            Localization = MainModel.Localization.CurrentLanguage.NonFictionDetailsTab;
            DetailsItem.UpdateLocalization(newLanguage);
        }
    }
}

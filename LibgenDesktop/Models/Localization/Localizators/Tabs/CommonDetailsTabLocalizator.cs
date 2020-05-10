using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators.Tabs
{
    internal class CommonDetailsTabLocalizator : DetailsTabLocalizator<Translation.DetailsTabsTranslation>
    {
        public CommonDetailsTabLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter, translation => translation?.DetailsTabs)
        {
            CoverIsLoading = Format(section => section?.CoverIsLoading);
            NoCover = Format(section => section?.NoCover);
            NoCoverMirror = Format(section => section?.NoCoverMirror);
            NoCoverDueToOfflineMode = Format(section => section?.NoCoverDueToOfflineMode);
            CoverLoadingError = Format(section => section?.CoverLoadingError);
            Download = Format(section => section?.Download);
            Queued = Format(section => section?.Queued);
            Downloading = Format(section => section?.Downloading);
            Stopped = Format(section => section?.Stopped);
            Error = Format(section => section?.Error);
            Open = Format(section => section?.Open);
            ErrorMessageTitle = Format(section => section?.ErrorMessageTitle);
            NoDownloadMirrorTooltip = Format(section => section?.NoDownloadMirrorTooltip);
            OfflineModeIsOnTooltip = Format(section => section?.OfflineModeIsOnTooltip);
        }

        public string CoverIsLoading { get; }
        public string NoCover { get; }
        public string NoCoverMirror { get; }
        public string NoCoverDueToOfflineMode { get; }
        public string CoverLoadingError { get; }
        public string Download { get; }
        public string Queued { get; }
        public string Downloading { get; }
        public string Stopped { get; }
        public string Error { get; }
        public string Open { get; }
        public string ErrorMessageTitle { get; }
        public string NoDownloadMirrorTooltip { get; }
        public string OfflineModeIsOnTooltip { get; }

        public string GetDownloadFromMirrorText(string mirror) => Format(section => section?.DownloadFromMirror, new { mirror });

        public string GetFileNotFoundErrorText(string file) => Format(section => section?.FileNotFoundError, new { file });
    }
}

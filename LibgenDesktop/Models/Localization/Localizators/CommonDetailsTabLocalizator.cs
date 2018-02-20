using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators
{
    internal class CommonDetailsTabLocalizator : DetailsTabLocalizator
    {
        public CommonDetailsTabLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter)
        {
            CoverIsLoading = Format(translation => translation?.CoverIsLoading);
            NoCover = Format(translation => translation?.NoCover);
            NoCoverMirror = Format(translation => translation?.NoCoverMirror);
            NoCoverDueToOfflineMode = Format(translation => translation?.NoCoverDueToOfflineMode);
            CoverLoadingError = Format(translation => translation?.CoverLoadingError);
            Download = Format(translation => translation?.Download);
            Queued = Format(translation => translation?.Queued);
            Downloading = Format(translation => translation?.Downloading);
            Stopped = Format(translation => translation?.Stopped);
            Error = Format(translation => translation?.Error);
            Open = Format(translation => translation?.Open);
            ErrorMessageTitle = Format(translation => translation?.ErrorMessageTitle);
            NoDownloadMirrorTooltip = Format(translation => translation?.NoDownloadMirrorTooltip);
            OfflineModeIsOnTooltip = Format(translation => translation?.OfflineModeIsOnTooltip);
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

        public string GetDownloadFromMirrorText(string mirror) => Format(translation => translation?.DownloadFromMirror, new { mirror });
        public string GetFileNotFoundErrorText(string file) => Format(translation => translation?.FileNotFoundError, new { file });
    }
}

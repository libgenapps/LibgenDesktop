using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators
{
    internal abstract class SearchResultsTabLocalizator : Localizator
    {
        public SearchResultsTabLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter)
        {
            SearchPlaceHolder = Format(translation => translation?.SearchPlaceHolder);
            SearchInProgress = Format(translation => translation?.SearchInProgress);
            Interrupt = Format(translation => translation?.Interrupt);
            Interrupting = Format(translation => translation?.Interrupting);
            AddToBookmarksTooltip = Format(translation => translation?.AddToBookmarksTooltip);
            RemoveFromBookmarksTooltip = Format(translation => translation?.RemoveFromBookmarksTooltip);
            ExportButtonTooltip = Format(translation => translation?.ExportButtonTooltip);
            Details = Format(translation => translation?.Details);
            Open = Format(translation => translation?.Open);
            Download = Format(translation => translation?.Download);
            ErrorMessageTitle = Format(translation => translation?.ErrorMessageTitle);
            OfflineModeIsOnMessageTitle = Format(translation => translation?.OfflineModeIsOnMessageTitle);
            OfflineModeIsOnMessageText = Format(translation => translation?.OfflineModeIsOnMessageText);
            NoDownloadMirrorError = Format(translation => translation?.NoDownloadMirrorError);
            LargeNumberOfItemsToDownloadPromptTitle = Format(translation => translation?.LargeNumberOfItemsToDownloadPromptTitle);
        }

        public string SearchPlaceHolder { get; }
        public string SearchInProgress { get; }
        public string Interrupt { get; }
        public string Interrupting { get; }
        public string AddToBookmarksTooltip { get; }
        public string RemoveFromBookmarksTooltip { get; }
        public string ExportButtonTooltip { get; }
        public string Details { get; }
        public string Open { get; }
        public string Download { get; }
        public string ErrorMessageTitle { get; }
        public string OfflineModeIsOnMessageTitle { get; }
        public string OfflineModeIsOnMessageText { get; }
        public string NoDownloadMirrorError { get; }
        public string LargeNumberOfItemsToDownloadPromptTitle { get; }

        public string GetFileNotFoundErrorText(string file) => Format(translation => translation?.FileNotFoundError, new { file });

        public string GetLargeNumberOfItemsToDownloadPromptText(int number) =>
            Format(translation => translation?.LargeNumberOfItemsToDownloadPromptText, new { number });

        private string Format(Func<Translation.SearchResultsTabsTranslation, string> field, object templateArguments = null)
        {
            return Format(translation => field(translation?.SearchResultsTabs), templateArguments);
        }
    }
}

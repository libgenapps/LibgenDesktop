using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators.Tabs
{
    internal abstract class SearchResultsTabLocalizator : Localizator<Translation.SearchResultsTabsTranslation>
    {
        public SearchResultsTabLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter, translation => translation?.SearchResultsTabs)
        {
            SearchPlaceHolder = Format(section => section?.SearchPlaceHolder);
            SearchInProgress = Format(section => section?.SearchInProgress);
            Interrupt = Format(section => section?.Interrupt);
            Interrupting = Format(section => section?.Interrupting);
            AddToBookmarksTooltip = Format(section => section?.AddToBookmarksTooltip);
            RemoveFromBookmarksTooltip = Format(section => section?.RemoveFromBookmarksTooltip);
            ExportButtonTooltip = Format(section => section?.ExportButtonTooltip);
            Details = Format(section => section?.Details);
            Open = Format(section => section?.Open);
            Download = Format(section => section?.Download);
            Copy = Format(section => section?.Copy);
            ErrorMessageTitle = Format(section => section?.ErrorMessageTitle);
            OfflineModeIsOnMessageTitle = Format(section => section?.OfflineModeIsOnMessageTitle);
            OfflineModeIsOnMessageText = Format(section => section?.OfflineModeIsOnMessageText);
            NoDownloadMirrorError = Format(section => section?.NoDownloadMirrorError);
            LargeNumberOfItemsToDownloadPromptTitle = Format(section => section?.LargeNumberOfItemsToDownloadPromptTitle);
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
        public string Copy { get; }
        public string ErrorMessageTitle { get; }
        public string OfflineModeIsOnMessageTitle { get; }
        public string OfflineModeIsOnMessageText { get; }
        public string NoDownloadMirrorError { get; }
        public string LargeNumberOfItemsToDownloadPromptTitle { get; }

        public string GetFileNotFoundErrorText(string file) => Format(section => section?.FileNotFoundError, new { file });

        public string GetLargeNumberOfItemsToDownloadPromptText(int number) =>
            Format(section => section?.LargeNumberOfItemsToDownloadPromptText, new { number = Formatter.ToFormattedString(number) });
    }

    internal abstract class SearchResultsTabLocalizator<T> : SearchResultsTabLocalizator where T : class
    {
        private readonly Func<Translation, T> searchResultsTabTranslationSectionSelector;

        public SearchResultsTabLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter,
            Func<Translation, T> searchResultsTabTranslationSectionSelector)
            : base(prioritizedTranslationList, formatter)
        {
            this.searchResultsTabTranslationSectionSelector = searchResultsTabTranslationSectionSelector;
        }

        protected string Format(Func<T, string> searchResultsTranslationSectionFieldSelector, object templateArguments = null) =>
            Format(translation => searchResultsTranslationSectionFieldSelector(searchResultsTabTranslationSectionSelector(translation)), templateArguments);
    }
}

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
        }

        public string SearchPlaceHolder { get; }
        public string SearchInProgress { get; }
        public string Interrupt { get; }
        public string Interrupting { get; }
        public string AddToBookmarksTooltip { get; }
        public string RemoveFromBookmarksTooltip { get; }
        public string ExportButtonTooltip { get; }

        private string Format(Func<Translation.SearchResultsTabsTranslation, string> field, object templateArguments = null)
        {
            return Format(translation => field(translation?.SearchResultsTabs), templateArguments);
        }
    }
}

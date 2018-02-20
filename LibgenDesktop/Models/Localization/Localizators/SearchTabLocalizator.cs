using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators
{
    internal class SearchTabLocalizator : Localizator
    {
        public SearchTabLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter)
        {
            TabTitle = Format(translation => translation?.TabTitle);
            NonFictionSelector = Format(translation => translation?.NonFictionSelector);
            SearchPlaceHolder = Format(translation => translation?.SearchPlaceHolder);
            NonFictionSelector = Format(translation => translation?.NonFictionSelector);
            FictionSelector = Format(translation => translation?.FictionSelector);
            SciMagSelector = Format(translation => translation?.SciMagSelector);
            NonFictionSearchBoxTooltip = Format(translation => translation?.NonFictionSearchBoxTooltip);
            FictionSearchBoxTooltip = Format(translation => translation?.FictionSearchBoxTooltip);
            SciMagSearchBoxTooltip = Format(translation => translation?.SciMagSearchBoxTooltip);
            SearchInProgress = Format(translation => translation?.SearchInProgress);
            DatabaseIsEmpty = Format(translation => translation?.DatabaseIsEmpty);
            ImportButton = Format(translation => translation?.ImportButton);
        }

        public string TabTitle { get; }
        public string SearchPlaceHolder { get; }
        public string NonFictionSelector { get; }
        public string FictionSelector { get; }
        public string SciMagSelector { get; }
        public string NonFictionSearchBoxTooltip { get; }
        public string FictionSearchBoxTooltip { get; }
        public string SciMagSearchBoxTooltip { get; }
        public string SearchInProgress { get; }
        public string DatabaseIsEmpty { get; }
        public string ImportButton { get; }

        public string GetNonFictionSearchProgressText(int count) =>
            Format(translation => translation?.NonFictionSearchProgress, new { count = Formatter.ToFormattedString(count) });
        public string GetFictionSearchProgressText(int count) =>
            Format(translation => translation?.FictionSearchProgress, new { count = Formatter.ToFormattedString(count) });
        public string GetSciMagSearchProgressText(int count) =>
            Format(translation => translation?.SciMagSearchProgress, new { count = Formatter.ToFormattedString(count) });

        private string Format(Func<Translation.SearchTabTranslation, string> field, object templateArguments = null)
        {
            return Format(translation => field(translation?.SearchTab), templateArguments);
        }
    }
}

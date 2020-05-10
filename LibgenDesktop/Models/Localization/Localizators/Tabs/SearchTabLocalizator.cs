using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators.Tabs
{
    internal class SearchTabLocalizator : Localizator<Translation.SearchTabTranslation>
    {
        public SearchTabLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter, translation => translation?.SearchTab)
        {
            TabTitle = Format(section => section?.TabTitle);
            NonFictionSelector = Format(section => section?.NonFictionSelector);
            SearchPlaceHolder = Format(section => section?.SearchPlaceHolder);
            NonFictionSelector = Format(section => section?.NonFictionSelector);
            FictionSelector = Format(section => section?.FictionSelector);
            SciMagSelector = Format(section => section?.SciMagSelector);
            NonFictionSearchBoxTooltip = Format(section => section?.NonFictionSearchBoxTooltip);
            FictionSearchBoxTooltip = Format(section => section?.FictionSearchBoxTooltip);
            SciMagSearchBoxTooltip = Format(section => section?.SciMagSearchBoxTooltip);
            SearchInProgress = Format(section => section?.SearchInProgress);
            Interrupt = Format(section => section?.Interrupt);
            Interrupting = Format(section => section?.Interrupting);
            DatabaseIsEmpty = Format(section => section?.DatabaseIsEmpty);
            ImportButton = Format(section => section?.ImportButton);
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
        public string Interrupt { get; }
        public string Interrupting { get; }
        public string DatabaseIsEmpty { get; }
        public string ImportButton { get; }

        public string GetNonFictionSearchProgressText(int count) =>
            Format(section => section?.NonFictionSearchProgress, new { count = Formatter.ToFormattedString(count) });

        public string GetFictionSearchProgressText(int count) =>
            Format(section => section?.FictionSearchProgress, new { count = Formatter.ToFormattedString(count) });

        public string GetSciMagSearchProgressText(int count) =>
            Format(section => section?.SciMagSearchProgress, new { count = Formatter.ToFormattedString(count) });
    }
}

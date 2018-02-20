using System;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Localization;

namespace LibgenDesktop.ViewModels.DetailsItems
{
    internal abstract class DetailsItemViewModel<T> : ViewModel where T : LibgenObject
    {
        public DetailsItemViewModel(T libgenObject, Language currentLanguage)
        {
            LibgenObject = libgenObject;
            CurrentLanguage = currentLanguage;
        }

        protected T LibgenObject { get; }
        protected Language CurrentLanguage { get; private set; }


        public void UpdateLocalization(Language newLanguage)
        {
            CurrentLanguage = newLanguage;
            UpdateLocalizableProperties();
        }

        protected abstract void UpdateLocalizableProperties();

        protected string FormatDateTime(DateTime dateTime)
        {
            return CurrentLanguage.Formatter.ToFormattedDateTimeString(dateTime);
        }

        protected string FormatFileSize(long fileSize)
        {
            return CurrentLanguage.Formatter.FileSizeToString(fileSize, true);
        }
    }
}

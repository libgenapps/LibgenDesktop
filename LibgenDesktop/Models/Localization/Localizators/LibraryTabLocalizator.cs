using System;
using System.Collections.Generic;
using LibgenDesktop.Models.ProgressArgs;

namespace LibgenDesktop.Models.Localization.Localizators
{
    internal class LibraryTabLocalizator : Localizator
    {
        public LibraryTabLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter)
        {
            TabTitle = Format(translation => translation?.TabTitle);
            ScanNonFiction = Format(translation => translation?.ScanNonFiction);
            ScanFiction = Format(translation => translation?.ScanFiction);
            ScanSciMag = Format(translation => translation?.ScanSciMag);
            BrowseDirectoryDialogTitle = Format(translation => translation?.BrowseDirectoryDialogTitle);
            CreatingIndexes = Format(translation => translation?.CreatingIndexes);
            ScanLog = Format(translation => translation?.ScanLog);
            AddAll = Format(translation => translation?.AddAll);
            Adding = Format(translation => translation?.Adding);
            Added = Format(translation => translation?.Added);
            Error = Format(translation => translation?.Error);
            ColumnsFile = Format(translation => translation?.File);
            ColumnsAuthors = Format(translation => translation?.Authors);
            ColumnsTitle = Format(translation => translation?.Title);
            ColumnsErrorType = Format(translation => translation?.ErrorType);
            ColumnsErrorDescription = Format(translation => translation?.ErrorDescription);
        }

        public string TabTitle { get; }
        public string ScanNonFiction { get; }
        public string ScanFiction { get; }
        public string ScanSciMag { get; }
        public string BrowseDirectoryDialogTitle { get; }
        public string CreatingIndexes { get; }
        public string AddAll { get; }
        public string Adding { get; }
        public string Added { get; }
        public string ScanLog { get; }
        public string Error { get; }
        public string ColumnsErrorType { get; }
        public string ColumnsErrorDescription { get; }
        public string ColumnsFile { get; }
        public string ColumnsAuthors { get; }
        public string ColumnsTitle { get; }

        public string GetScanStartedString(string directory) => Format(translation => translation?.ScanStarted, new { directory });
        public string GetFoundString(int count) => Format(translation => translation?.Found, new { count });
        public string GetNotFoundString(int count) => Format(translation => translation?.NotFound, new { count });
        public string GetErrorsString(int count) => Format(translation => translation?.Errors, new { count });
        public string GetErrorDescription(ErrorTypes errorType)
        {
            switch (errorType)
            {
                case ErrorTypes.ERROR_NONE:
                    return "";
                case ErrorTypes.ERROR_DIRECTORY_ACCESS:
                    return Format(translation => translation?.Library.ErrorDescriptions.DirectoryAccess);
                case ErrorTypes.ERROR_DIRECTORY_NOT_FOUND:
                    return Format(translation => translation?.Library.ErrorDescriptions.DirectoryNotFound);
                case ErrorTypes.ERROR_FILE_SIZE_ZERO:
                    return Format(translation => translation?.Library.ErrorDescriptions.FileSize);
                case ErrorTypes.ERROR_FILE_NOT_FOUND:
                    return Format(translation => translation?.Library.ErrorDescriptions.FileNotFound);
                case ErrorTypes.ERROR_FILE_PATH_TOO_LONG:
                    return Format(translation => translation?.Library.ErrorDescriptions.FilePathTooLong);
                case ErrorTypes.ERROR_FILE_ACCESS:
                    return Format(translation => translation?.Library.ErrorDescriptions.FileAccess);
                case ErrorTypes.ERROR_FILE_IN_USE:
                    return Format(translation => translation?.Library.ErrorDescriptions.FileInUse);
                case ErrorTypes.ERROR_IO_EXCEPTION:
                    return Format(translation => translation?.Library.ErrorDescriptions.IoException);
                case ErrorTypes.ERROR_MD5_HASH_NOT_IN_DB:
                    return Format(translation => translation?.Library.ErrorDescriptions.MD5HashError);
                case ErrorTypes.ERROR_OTHER:
                    return Format(translation => translation?.Library.ErrorDescriptions.OtherException);
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public string GetScanCompleteString(int found, int notFound, int errors) => Format(translation => translation?.ScanComplete,
            new { found = Formatter.ToFormattedString(found), notFound = Formatter.ToFormattedString(notFound),
                errors = Formatter.ToFormattedString(errors) });

        private string Format(Func<Translation.LibraryTranslation, string> field, object templateArguments = null)
        {
            return Format(translation => field(translation?.Library), templateArguments);
        }

        private string Format(Func<Translation.LibraryColumnsTranslation, string> field, object templateArguments = null)
        {
            return Format(translation => field(translation?.Library?.Columns), templateArguments);
        }
    }
}

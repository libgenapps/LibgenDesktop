namespace LibgenDesktop.Models.Localization
{
    internal class Translation
    {
        internal class GeneralInfo
        {
            public string Name { get; set; }
            public string LocalizedName { get; set; }
            public string CultureCode { get; set; }
            public string TranslatorName { get; set; }
        }

        internal class FileSizePostfixList
        {
            public string Byte { get; set; }
            public string Kilobyte { get; set; }
            public string Megabyte { get; set; }
            public string Gigabyte { get; set; }
            public string Terabyte { get; set; }
        }

        internal class FormattingInfo
        {
            public string DecimalSeparator { get; set; }
            public string ThousandsSeparator { get; set; }
            public string DateFormat { get; set; }
            public string TimeFormat { get; set; }
            public FileSizePostfixList FileSizePostfixes { get; set; }
        }

        internal class MainMenuTranslation
        {
            public string DownloadManagerTooltip { get; set; }
            public string BookmarksTooltip { get; set; }
            public string NoBookmarks { get; set; }
            public string Update { get; set; }
            public string Import { get; set; }
            public string Synchronize { get; set; }
            public string Library { get; set; }
            public string Database { get; set; }
            public string Settings { get; set; }
            public string About { get; set; }
        }

        internal class MainWindowTranslation
        {
            public string WindowTitle { get; set; }
            public MainMenuTranslation MainMenu { get; set; }
        }

        internal class CreateDatabaseWindowTranslation
        {
            public string WindowTitle { get; set; }
            public string FirstRunMessage { get; set; }
            public string DatabaseNotFound { get; set; }
            public string DatabaseCorrupted { get; set; }
            public string ChooseOption { get; set; }
            public string CreateNewDatabase { get; set; }
            public string OpenExistingDatabase { get; set; }
            public string BrowseNewDatabaseDialogTitle { get; set; }
            public string BrowseExistingDatabaseDialogTitle { get; set; }
            public string Databases { get; set; }
            public string AllFiles { get; set; }
            public string Error { get; set; }
            public string CannotCreateDatabase { get; set; }
            public string Ok { get; set; }
            public string Cancel { get; set; }
        }

        internal class SearchTabTranslation
        {
            public string TabTitle { get; set; }
            public string SearchPlaceHolder { get; set; }
            public string NonFictionSelector { get; set; }
            public string FictionSelector { get; set; }
            public string SciMagSelector { get; set; }
            public string NonFictionSearchBoxTooltip { get; set; }
            public string FictionSearchBoxTooltip { get; set; }
            public string SciMagSearchBoxTooltip { get; set; }
            public string SearchInProgress { get; set; }
            public string NonFictionSearchProgress { get; set; }
            public string FictionSearchProgress { get; set; }
            public string SciMagSearchProgress { get; set; }
            public string Interrupt { get; set; }
            public string Interrupting { get; set; }
            public string DatabaseIsEmpty { get; set; }
            public string ImportButton { get; set; }
        }

        internal class SearchResultsTabsTranslation
        {
            public string SearchPlaceHolder { get; set; }
            public string SearchInProgress { get; set; }
            public string Interrupt { get; set; }
            public string Interrupting { get; set; }
            public string AddToBookmarksTooltip { get; set; }
            public string RemoveFromBookmarksTooltip { get; set; }
            public string ExportButtonTooltip { get; set; }
        }

        internal class NonFictionSearchResultsGridColumnsTranslation
        {
            public string Title { get; set; }
            public string Authors { get; set; }
            public string Series { get; set; }
            public string Year { get; set; }
            public string Publisher { get; set; }
            public string Format { get; set; }
            public string FileSize { get; set; }
            public string Ocr { get; set; }
        }

        internal class NonFictionSearchResultsTabTranslation
        {
            public string SearchBoxTooltip { get; set; }
            public string SearchProgress { get; set; }
            public string StatusBar { get; set; }
            public NonFictionSearchResultsGridColumnsTranslation Columns { get; set; }
        }

        internal class FictionSearchResultsGridColumnsTranslation
        {
            public string Title { get; set; }
            public string Authors { get; set; }
            public string Series { get; set; }
            public string Year { get; set; }
            public string Publisher { get; set; }
            public string Format { get; set; }
            public string FileSize { get; set; }
        }

        internal class FictionSearchResultsTabTranslation
        {
            public string SearchBoxTooltip { get; set; }
            public string SearchProgress { get; set; }
            public string StatusBar { get; set; }
            public FictionSearchResultsGridColumnsTranslation Columns { get; set; }
        }

        internal class SciMagSearchResultsGridColumnsTranslation
        {
            public string Title { get; set; }
            public string Authors { get; set; }
            public string Magazine { get; set; }
            public string Year { get; set; }
            public string FileSize { get; set; }
            public string Doi { get; set; }
        }

        internal class SciMagSearchResultsTabTranslation
        {
            public string SearchBoxTooltip { get; set; }
            public string SearchProgress { get; set; }
            public string StatusBar { get; set; }
            public SciMagSearchResultsGridColumnsTranslation Columns { get; set; }
        }

        internal class DetailsTabsTranslation
        {
            public string CoverIsLoading { get; set; }
            public string NoCover { get; set; }
            public string NoCoverMirror { get; set; }
            public string NoCoverDueToOfflineMode { get; set; }
            public string CoverLoadingError { get; set; }
            public string Yes { get; set; }
            public string No { get; set; }
            public string Unknown { get; set; }
            public string Portrait { get; set; }
            public string Landscape { get; set; }
            public string CopyContextMenu { get; set; }
            public string Download { get; set; }
            public string DownloadFromMirror { get; set; }
            public string Queued { get; set; }
            public string Downloading { get; set; }
            public string Stopped { get; set; }
            public string Error { get; set; }
            public string Open { get; set; }
            public string ErrorMessageTitle { get; set; }
            public string FileNotFoundError { get; set; }
            public string NoDownloadMirrorTooltip { get; set; }
            public string OfflineModeIsOnTooltip { get; set; }
            public string Close { get; set; }
        }

        internal class NonFictionDetailsTabTranslation
        {
            public string Title { get; set; }
            public string Authors { get; set; }
            public string Series { get; set; }
            public string Publisher { get; set; }
            public string Year { get; set; }
            public string Language { get; set; }
            public string Format { get; set; }
            public string Isbn { get; set; }
            public string Added { get; set; }
            public string LastModified { get; set; }
            public string Library { get; set; }
            public string FileSize { get; set; }
            public string Topics { get; set; }
            public string Volume { get; set; }
            public string Magazine { get; set; }
            public string City { get; set; }
            public string Edition { get; set; }
            public string Pages { get; set; }
            public string BodyMatterPages { get; set; }
            public string TotalPages { get; set; }
            public string Tags { get; set; }
            public string Md5Hash { get; set; }
            public string Comments { get; set; }
            public string Identifiers { get; set; }
            public string LibgenId { get; set; }
            public string Issn { get; set; }
            public string Udc { get; set; }
            public string Lbc { get; set; }
            public string Lcc { get; set; }
            public string Ddc { get; set; }
            public string Doi { get; set; }
            public string OpenLibraryId { get; set; }
            public string GoogleBookId { get; set; }
            public string Asin { get; set; }
            public string AdditionalAttributes { get; set; }
            public string Dpi { get; set; }
            public string Ocr { get; set; }
            public string TableOfContents { get; set; }
            public string Scanned { get; set; }
            public string Orientation { get; set; }
            public string Paginated { get; set; }
            public string Colored { get; set; }
            public string Cleaned { get; set; }
        }

        internal class FictionDetailsTabTranslation
        {
            public string Title { get; set; }
            public string Authors { get; set; }
            public string RussianAuthor { get; set; }
            public string Series { get; set; }
            public string Publisher { get; set; }
            public string Edition { get; set; }
            public string Year { get; set; }
            public string Language { get; set; }
            public string Format { get; set; }
            public string Pages { get; set; }
            public string Version { get; set; }
            public string FileSize { get; set; }
            public string Added { get; set; }
            public string LastModified { get; set; }
            public string Md5Hash { get; set; }
            public string Comments { get; set; }
            public string Identifiers { get; set; }
            public string LibgenId { get; set; }
            public string Isbn { get; set; }
            public string GoogleBookId { get; set; }
            public string Asin { get; set; }
        }

        internal class SciMagDetailsTabTranslation
        {
            public string Title { get; set; }
            public string Authors { get; set; }
            public string Magazine { get; set; }
            public string Year { get; set; }
            public string Month { get; set; }
            public string Day { get; set; }
            public string Volume { get; set; }
            public string Issue { get; set; }
            public string Pages { get; set; }
            public string FileSize { get; set; }
            public string AddedDateTime { get; set; }
            public string Md5Hash { get; set; }
            public string AbstractUrl { get; set; }
            public string Identifiers { get; set; }
            public string LibgenId { get; set; }
            public string Doi { get; set; }
            public string Isbn { get; set; }
            public string MagazineId { get; set; }
            public string Issnp { get; set; }
            public string Issne { get; set; }
            public string PubmedId { get; set; }
            public string Pmc { get; set; }
            public string Pii { get; set; }
            public string AdditionalAttributes { get; set; }
            public string Attribute1 { get; set; }
            public string Attribute2 { get; set; }
            public string Attribute3 { get; set; }
            public string Attribute4 { get; set; }
            public string Attribute5 { get; set; }
            public string Attribute6 { get; set; }
        }

        internal class ImportStatusMessagesTranslation
        {
            public string Step { get; set; }
            public string DataLookup { get; set; }
            public string CreatingIndexes { get; set; }
            public string LoadingIds { get; set; }
            public string ImportingData { get; set; }
            public string ImportComplete { get; set; }
            public string ImportCancelled { get; set; }
            public string DataNotFound { get; set; }
            public string ImportError { get; set; }
        }

        internal class ImportLogMessagesTranslation
        {
            public string Step { get; set; }
            public string DataLookup { get; set; }
            public string Scanning { get; set; }
            public string ScannedProgress { get; set; }
            public string NonFictionTableFound { get; set; }
            public string FictionTableFound { get; set; }
            public string SciMagTableFound { get; set; }
            public string CreatingIndexes { get; set; }
            public string CreatingIndexForColumn { get; set; }
            public string LoadingIds { get; set; }
            public string LoadingColumnValues { get; set; }
            public string ImportingData { get; set; }
            public string ImportBooksProgressNoUpdate { get; set; }
            public string ImportBooksProgressWithUpdate { get; set; }
            public string ImportArticlesProgressNoUpdate { get; set; }
            public string ImportArticlesProgressWithUpdate { get; set; }
            public string ImportSuccessful { get; set; }
            public string ImportCancelled { get; set; }
            public string DataNotFound { get; set; }
            public string ImportError { get; set; }
        }

        internal class ImportTranslation
        {
            public string WindowTitle { get; set; }
            public string BrowseImportFileDialogTitle { get; set; }
            public string AllSupportedFiles { get; set; }
            public string SqlDumps { get; set; }
            public string Archives { get; set; }
            public string AllFiles { get; set; }
            public string Elapsed { get; set; }
            public string Interrupt { get; set; }
            public string Interrupting { get; set; }
            public string Close { get; set; }
            public ImportStatusMessagesTranslation StatusMessages { get; set; }
            public ImportLogMessagesTranslation LogMessages { get; set; }
        }

        internal class ExportPanelTranslation
        {
            public string Header { get; set; }
            public string Format { get; set; }
            public string Excel { get; set; }
            public string Csv { get; set; }
            public string Separator { get; set; }
            public string Comma { get; set; }
            public string Semicolon { get; set; }
            public string Tab { get; set; }
            public string SaveAs { get; set; }
            public string Browse { get; set; }
            public string BrowseDialogTitle { get; set; }
            public string ExcelFiles { get; set; }
            public string CsvFiles { get; set; }
            public string TsvFiles { get; set; }
            public string AllFiles { get; set; }
            public string ExportRange { get; set; }
            public string NoLimit { get; set; }
            public string Limit { get; set; }
            public string Export { get; set; }
            public string Cancel { get; set; }
            public string SavingFile { get; set; }
            public string RowCountSingleFile { get; set; }
            public string RowCountMultipleFiles { get; set; }
            public string ErrorWarningTitle { get; set; }
            public string InvalidExportPath { get; set; }
            public string DirectoryNotFound { get; set; }
            public string InvalidExportFileName { get; set; }
            public string OverwritePromptTitle { get; set; }
            public string OverwritePromptText { get; set; }
            public string RowLimitWarningTitle { get; set; }
            public string RowLimitWarningText { get; set; }
            public string ExportError { get; set; }
            public string Interrupt { get; set; }
            public string Interrupting { get; set; }
            public string ExportInterrupted { get; set; }
            public string Results { get; set; }
            public string Close { get; set; }
        }

        internal class NonFictionExporterColumnsTranslation
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public string Authors { get; set; }
            public string Series { get; set; }
            public string Publisher { get; set; }
            public string Year { get; set; }
            public string Language { get; set; }
            public string Format { get; set; }
            public string Isbn { get; set; }
            public string Added { get; set; }
            public string LastModified { get; set; }
            public string Library { get; set; }
            public string FileSize { get; set; }
            public string Topics { get; set; }
            public string Volume { get; set; }
            public string Magazine { get; set; }
            public string City { get; set; }
            public string Edition { get; set; }
            public string BodyMatterPages { get; set; }
            public string TotalPages { get; set; }
            public string Tags { get; set; }
            public string Md5Hash { get; set; }
            public string Comments { get; set; }
            public string LibgenId { get; set; }
            public string Issn { get; set; }
            public string Udc { get; set; }
            public string Lbc { get; set; }
            public string Lcc { get; set; }
            public string Ddc { get; set; }
            public string Doi { get; set; }
            public string OpenLibraryId { get; set; }
            public string GoogleBookId { get; set; }
            public string Asin { get; set; }
            public string Dpi { get; set; }
            public string Ocr { get; set; }
            public string TableOfContents { get; set; }
            public string Scanned { get; set; }
            public string Orientation { get; set; }
            public string Paginated { get; set; }
            public string Colored { get; set; }
            public string Cleaned { get; set; }
        }

        internal class FictionExporterColumnsTranslation
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public string Authors { get; set; }
            public string RussianAuthor { get; set; }
            public string Series { get; set; }
            public string Publisher { get; set; }
            public string Edition { get; set; }
            public string Year { get; set; }
            public string Language { get; set; }
            public string Format { get; set; }
            public string Pages { get; set; }
            public string Version { get; set; }
            public string FileSize { get; set; }
            public string Added { get; set; }
            public string LastModified { get; set; }
            public string Md5Hash { get; set; }
            public string Comments { get; set; }
            public string LibgenId { get; set; }
            public string Isbn { get; set; }
            public string GoogleBookId { get; set; }
            public string Asin { get; set; }
        }

        internal class SciMagExporterColumnsTranslation
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public string Authors { get; set; }
            public string Magazine { get; set; }
            public string Year { get; set; }
            public string Month { get; set; }
            public string Day { get; set; }
            public string Volume { get; set; }
            public string Issue { get; set; }
            public string Pages { get; set; }
            public string FileSize { get; set; }
            public string AddedDateTime { get; set; }
            public string Md5Hash { get; set; }
            public string AbstractUrl { get; set; }
            public string LibgenId { get; set; }
            public string Doi1 { get; set; }
            public string Doi2 { get; set; }
            public string Isbn { get; set; }
            public string MagazineId { get; set; }
            public string Issnp { get; set; }
            public string Issne { get; set; }
            public string PubmedId { get; set; }
            public string Pmc { get; set; }
            public string Pii { get; set; }
            public string Attribute1 { get; set; }
            public string Attribute2 { get; set; }
            public string Attribute3 { get; set; }
            public string Attribute4 { get; set; }
            public string Attribute5 { get; set; }
            public string Attribute6 { get; set; }
        }

        internal class ExporterTranslation
        {
            public string Yes { get; set; }
            public string No { get; set; }
            public string Unknown { get; set; }
            public string Portrait { get; set; }
            public string Landscape { get; set; }
            public NonFictionExporterColumnsTranslation NonFictionColumns { get; set; }
            public FictionExporterColumnsTranslation FictionColumns { get; set; }
            public SciMagExporterColumnsTranslation SciMagColumns { get; set; }
        }

        internal class SynchronizationStatusMessagesTranslation
        {
            public string Step { get; set; }
            public string Preparation { get; set; }
            public string CreatingIndexes { get; set; }
            public string LoadingIds { get; set; }
            public string SynchronizingData { get; set; }
            public string SynchronizationComplete { get; set; }
            public string SynchronizationCancelled { get; set; }
            public string SynchronizationError { get; set; }
        }

        internal class SynchronizationLogMessagesTranslation
        {
            public string Step { get; set; }
            public string CreatingIndexes { get; set; }
            public string CreatingIndexForColumn { get; set; }
            public string LoadingIds { get; set; }
            public string LoadingColumnValues { get; set; }
            public string SynchronizingBookList { get; set; }
            public string DownloadingNewBooks { get; set; }
            public string SynchronizationProgressNoAddedNoUpdated { get; set; }
            public string SynchronizationProgressAdded { get; set; }
            public string SynchronizationProgressUpdated { get; set; }
            public string SynchronizationProgressAddedAndUpdated { get; set; }
            public string SynchronizationSuccessful { get; set; }
            public string SynchronizationCancelled { get; set; }
            public string SynchronizationError { get; set; }
        }

        internal class SynchronizationTranslation
        {
            public string WindowTitle { get; set; }
            public string ErrorMessageTitle { get; set; }
            public string ImportRequired { get; set; }
            public string NoSynchronizationMirror { get; set; }
            public string OfflineModePromptTitle { get; set; }
            public string OfflineModePromptText { get; set; }
            public string Elapsed { get; set; }
            public string Interrupt { get; set; }
            public string Interrupting { get; set; }
            public string Close { get; set; }
            public SynchronizationStatusMessagesTranslation StatusMessages { get; set; }
            public SynchronizationLogMessagesTranslation LogMessages { get; set; }
        }

        internal class LibraryTranslation
        {
            public string WindowTitle { get; set; }
            public string Scan { get; set; }
            public string BrowseDirectoryDialogTitle { get; set; }
            public string ScanStarted { get; set; }
            public string CreatingIndexes { get; set; }
            public string NotFound { get; set; }
            public string ScanComplete { get; set; }
        }

        internal class DatabaseTranslation
        {
            public string WindowTitle { get; set; }
            public string NonFiction { get; set; }
            public string Fiction { get; set; }
            public string SciMagArticles { get; set; }
            public string TotalBooks { get; set; }
            public string TotalArticles { get; set; }
            public string LastUpdate { get; set; }
            public string Never { get; set; }
            public string CreatingIndexes { get; set; }
            public string ChangeDatabase { get; set; }
            public string BrowseDatabaseDialogTitle { get; set; }
            public string Databases { get; set; }
            public string AllFiles { get; set; }
            public string Error { get; set; }
            public string CannotOpenDatabase { get; set; }
            public string Close { get; set; }
        }

        internal class DownloadManagerLogMessagesTranslation
        {
            public string Queued { get; set; }
            public string Started { get; set; }
            public string Stopped { get; set; }
            public string RetryDelay { get; set; }
            public string Completed { get; set; }
            public string OfflineModeIsOn { get; set; }
            public string TransformationError { get; set; }
            public string TransformationReturnedIncorrectUrl { get; set; }
            public string Attempt { get; set; }
            public string MaximumDownloadAttempts { get; set; }
            public string DownloadingPage { get; set; }
            public string DownloadingFile { get; set; }
            public string StartingFileDownloadKnownFileSize { get; set; }
            public string StartingFileDownloadUnknownFileSize { get; set; }
            public string ResumingFileDownloadKnownFileSize { get; set; }
            public string ResumingFileDownloadUnknownFileSize { get; set; }
            public string Request { get; set; }
            public string Response { get; set; }
            public string Redirect { get; set; }
            public string TooManyRedirects { get; set; }
            public string NonSuccessfulStatusCode { get; set; }
            public string CannotCreateDownloadDirectory { get; set; }
            public string CannotCreateOrOpenFile { get; set; }
            public string CannotRenamePartFile { get; set; }
            public string HtmlPageReturned { get; set; }
            public string NoPartialDownloadSupport { get; set; }
            public string NoContentLengthWarning { get; set; }
            public string ServerResponseTimeout { get; set; }
            public string DownloadIncompleteError { get; set; }
            public string FileWriteError { get; set; }
            public string UnexpectedError { get; set; }
        }

        internal class DownloadManagerTranslation
        {
            public string TabTitle { get; set; }
            public string Start { get; set; }
            public string Stop { get; set; }
            public string Remove { get; set; }
            public string StartAll { get; set; }
            public string StopAll { get; set; }
            public string RemoveCompleted { get; set; }
            public string QueuedStatus { get; set; }
            public string DownloadingStatus { get; set; }
            public string StoppedStatus { get; set; }
            public string RetryDelayStatus { get; set; }
            public string ErrorStatus { get; set; }
            public string DownloadProgressKnownFileSize { get; set; }
            public string DownloadProgressUnknownFileSize { get; set; }
            public string Log { get; set; }
            public string TechnicalDetails { get; set; }
            public string Copy { get; set; }
            public string FileNotFoundErrorTitle { get; set; }
            public string FileNotFoundErrorText { get; set; }
            public DownloadManagerLogMessagesTranslation LogMessages { get; set; }
        }

        internal class ApplicationUpdateTranslation
        {
            public string WindowTitle { get; set; }
            public string UpdateAvailable { get; set; }
            public string NewVersion { get; set; }
            public string Download { get; set; }
            public string DownloadAndInstall { get; set; }
            public string SkipThisVersion { get; set; }
            public string Cancel { get; set; }
            public string Interrupt { get; set; }
            public string Interrupting { get; set; }
            public string InterruptPromptTitle { get; set; }
            public string InterruptPromptText { get; set; }
            public string Error { get; set; }
            public string IncompleteDownload { get; set; }
            public string Close { get; set; }
        }

        internal class UpdateCheckIntervalTranslation
        {
            public string Never { get; set; }
            public string Daily { get; set; }
            public string Weekly { get; set; }
            public string Monthly { get; set; }
        }

        internal class GeneralSettingsTranslation
        {
            public string TabHeader { get; set; }
            public string Language { get; set; }
            public string CheckUpdates { get; set; }
            public UpdateCheckIntervalTranslation UpdateCheckIntervals { get; set; }
        }

        internal class NetworkSettingsTranslation
        {
            public string TabHeader { get; set; }
            public string OfflineMode { get; set; }
            public string UseHttpProxy { get; set; }
            public string ProxyAddress { get; set; }
            public string ProxyAddressRequired { get; set; }
            public string ProxyPort { get; set; }
            public string ProxyPortValidation { get; set; }
            public string ProxyUserName { get; set; }
            public string ProxyPassword { get; set; }
            public string ProxyPasswordWarning { get; set; }
        }

        internal class DownloadSettingsTranslation
        {
            public string TabHeader { get; set; }
            public string DownloadMode { get; set; }
            public string OpenInBrowser { get; set; }
            public string UseDownloadManager { get; set; }
            public string DownloadDirectory { get; set; }
            public string BrowseDirectoryDialogTitle { get; set; }
            public string DownloadDirectoryNotFound { get; set; }
            public string Timeout { get; set; }
            public string TimeoutValidation { get; set; }
            public string Seconds { get; set; }
            public string DownloadAttempts { get; set; }
            public string DownloadAttemptsValidation { get; set; }
            public string Times { get; set; }
            public string RetryDelay { get; set; }
            public string RetryDelayValidation { get; set; }
        }

        internal class MirrorsSettingsTranslation
        {
            public string TabHeader { get; set; }
            public string NonFiction { get; set; }
            public string Fiction { get; set; }
            public string SciMagArticles { get; set; }
            public string Books { get; set; }
            public string Articles { get; set; }
            public string Covers { get; set; }
            public string Synchronization { get; set; }
            public string NoMirror { get; set; }
        }

        internal class SearchSettingsTranslation
        {
            public string TabHeader { get; set; }
            public string LimitResults { get; set; }
            public string MaximumResults { get; set; }
            public string PositiveNumbersOnly { get; set; }
            public string OpenDetails { get; set; }
            public string InModalWindow { get; set; }
            public string InNonModalWindow { get; set; }
            public string InNewTab { get; set; }
        }

        internal class ExportSettingsTranslation
        {
            public string TabHeader { get; set; }
            public string OpenResults { get; set; }
            public string SplitIntoMultipleFiles { get; set; }
            public string MaximumRowsPerFile { get; set; }
            public string MaximumRowsPerFileValidation { get; set; }
            public string ExcelLimitNote { get; set; }
        }

        internal class AdvancedSettingsTranslation
        {
            public string TabHeader { get; set; }
            public string UseLogging { get; set; }
        }

        internal class SettingsTranslation
        {
            public string WindowTitle { get; set; }
            public string Ok { get; set; }
            public string Cancel { get; set; }
            public string DiscardChangesPromptTitle { get; set; }
            public string DiscardChangesPromptText { get; set; }
            public GeneralSettingsTranslation General { get; set; }
            public NetworkSettingsTranslation Network { get; set; }
            public DownloadSettingsTranslation Download { get; set; }
            public MirrorsSettingsTranslation Mirrors { get; set; }
            public SearchSettingsTranslation Search { get; set; }
            public ExportSettingsTranslation Export { get; set; }
            public AdvancedSettingsTranslation Advanced { get; set; }
        }

        internal class AboutTranslation
        {
            public string WindowTitle { get; set; }
            public string ApplicationName { get; set; }
            public string Version { get; set; }
            public string CheckForUpdates { get; set; }
            public string CheckingUpdates { get; set; }
            public string OfflineModeIsOnTooltip { get; set; }
            public string LatestVersion { get; set; }
            public string NewVersionAvailable { get; set; }
            public string Update { get; set; }
            public string Translators { get; set; }
        }

        internal class MessageBoxTranslation
        {
            public string Ok { get; set; }
            public string Yes { get; set; }
            public string No { get; set; }
        }

        internal class ErrorWindowTranslation
        {
            public string WindowTitle { get; set; }
            public string UnexpectedError { get; set; }
            public string Copy { get; set; }
            public string Close { get; set; }
        }

        public GeneralInfo General { get; set; }
        public FormattingInfo Formatting { get; set; }
        public MainWindowTranslation MainWindow { get; set; }
        public CreateDatabaseWindowTranslation CreateDatabaseWindow { get; set; }
        public SearchTabTranslation SearchTab { get; set; }
        public SearchResultsTabsTranslation SearchResultsTabs { get; set; }
        public NonFictionSearchResultsTabTranslation NonFictionSearchResultsTab { get; set; }
        public FictionSearchResultsTabTranslation FictionSearchResultsTab { get; set; }
        public SciMagSearchResultsTabTranslation SciMagSearchResultsTab { get; set; }
        public DetailsTabsTranslation DetailsTabs { get; set; }
        public NonFictionDetailsTabTranslation NonFictionDetailsTab { get; set; }
        public FictionDetailsTabTranslation FictionDetailsTab { get; set; }
        public SciMagDetailsTabTranslation SciMagDetailsTab { get; set; }
        public ImportTranslation Import { get; set; }
        public ExportPanelTranslation ExportPanel { get; set; }
        public ExporterTranslation Exporter { get; set; }
        public SynchronizationTranslation Synchronization { get; set; }
        public DownloadManagerTranslation DownloadManager { get; set; }
        public ApplicationUpdateTranslation ApplicationUpdate { get; set; }
        public LibraryTranslation Library { get; set; }
        public DatabaseTranslation Database { get; set; }
        public SettingsTranslation Settings { get; set; }
        public AboutTranslation About { get; set; }
        public MessageBoxTranslation MessageBox { get; set; }
        public ErrorWindowTranslation ErrorWindow { get; set; }
    }
}

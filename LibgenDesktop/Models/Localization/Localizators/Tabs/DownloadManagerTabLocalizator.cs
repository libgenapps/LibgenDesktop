using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators.Tabs
{
    internal class DownloadManagerTabLocalizator : Localizator<Translation.DownloadManagerTranslation>
    {
        public DownloadManagerTabLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter, translation => translation?.DownloadManager)
        {
            TabTitle = Format(section => section?.TabTitle);
            Start = Format(section => section?.Start);
            Stop = Format(section => section?.Stop);
            Remove = Format(section => section?.Remove);
            StartAll = Format(section => section?.StartAll);
            StopAll = Format(section => section?.StopAll);
            RemoveCompleted = Format(section => section?.RemoveCompleted);
            QueuedStatus = Format(section => section?.QueuedStatus);
            DownloadingStatus = Format(section => section?.DownloadingStatus);
            StoppedStatus = Format(section => section?.StoppedStatus);
            RetryDelayStatus = Format(section => section?.RetryDelayStatus);
            ErrorStatus = Format(section => section?.ErrorStatus);
            Log = Format(section => section?.Log);
            TechnicalDetails = Format(section => section?.TechnicalDetails);
            Copy = Format(section => section?.Copy);
            FileNotFoundErrorTitle = Format(section => section?.FileNotFoundErrorTitle);
            LogLineQueued = Format(section => section?.LogMessages?.Queued);
            LogLineStarted = Format(section => section?.LogMessages?.Started);
            LogLineStopped = Format(section => section?.LogMessages?.Stopped);
            LogLineCompleted = Format(section => section?.LogMessages?.Completed);
            LogLineOfflineModeIsOn = Format(section => section?.LogMessages?.OfflineModeIsOn);
            LogLineMaximumDownloadAttempts = Format(section => section?.LogMessages?.MaximumDownloadAttempts);
            LogLineStartingFileDownloadUnknownFileSize = Format(section => section?.LogMessages?.StartingFileDownloadUnknownFileSize);
            LogLineResumingFileDownloadUnknownFileSize = Format(section => section?.LogMessages?.ResumingFileDownloadUnknownFileSize);
            LogLineRequest = Format(section => section?.LogMessages?.Request);
            LogLineResponse = Format(section => section?.LogMessages?.Response);
            LogLineTooManyRedirects = Format(section => section?.LogMessages?.TooManyRedirects);
            LogLineHtmlPageReturned = Format(section => section?.LogMessages?.HtmlPageReturned);
            LogLineNoPartialDownloadSupport = Format(section => section?.LogMessages?.NoPartialDownloadSupport);
            LogLineNoContentLengthWarning = Format(section => section?.LogMessages?.NoContentLengthWarning);
            LogLineServerResponseTimeout = Format(section => section?.LogMessages?.ServerResponseTimeout);
            LogLineDownloadIncompleteError = Format(section => section?.LogMessages?.DownloadIncompleteError);
            LogLineFileWriteError = Format(section => section?.LogMessages?.FileWriteError);
        }

        public string TabTitle { get; }
        public string Start { get; }
        public string Stop { get; }
        public string Remove { get; }
        public string StartAll { get; }
        public string StopAll { get; }
        public string RemoveCompleted { get; }
        public string QueuedStatus { get; }
        public string DownloadingStatus { get; }
        public string StoppedStatus { get; }
        public string RetryDelayStatus { get; }
        public string ErrorStatus { get; }
        public string Log { get; }
        public string TechnicalDetails { get; }
        public string Copy { get; }
        public string FileNotFoundErrorTitle { get; }
        public string LogLineQueued { get; }
        public string LogLineStarted { get; }
        public string LogLineStopped { get; }
        public string LogLineCompleted { get; }
        public string LogLineOfflineModeIsOn { get; }
        public string LogLineMaximumDownloadAttempts { get; }
        public string LogLineStartingFileDownloadUnknownFileSize { get; }
        public string LogLineResumingFileDownloadUnknownFileSize { get; }
        public string LogLineRequest { get; }
        public string LogLineResponse { get; }
        public string LogLineTooManyRedirects { get; }
        public string LogLineHtmlPageReturned { get; }
        public string LogLineNoPartialDownloadSupport { get; }
        public string LogLineNoContentLengthWarning { get; }
        public string LogLineServerResponseTimeout { get; }
        public string LogLineDownloadIncompleteError { get; }
        public string LogLineFileWriteError { get; }

        public string GetDownloadProgressKnownFileSize(long downloaded, long total, int percent) =>
            Format(section => section?.DownloadProgressKnownFileSize,
                new { downloaded = Formatter.ToFormattedString(downloaded), total = Formatter.ToFormattedString(total),
                    percent = Formatter.ToFormattedString(percent) });

        public string GetDownloadProgressUnknownFileSize(long downloaded) =>
            Format(section => section?.DownloadProgressUnknownFileSize, new { downloaded = Formatter.ToFormattedString(downloaded) });

        public string GetFileNotFoundErrorText(string file) => Format(section => section?.FileNotFoundErrorText, new { file });

        public string GetLogLineRetryDelay(int count) =>
            Format(section => section?.LogMessages?.RetryDelay, new { count = Formatter.ToFormattedString(count) });

        public string GetLogLineTransformationError(string transformation) =>
            Format(section => section?.LogMessages?.TransformationError, new { transformation });

        public string GetLogLineTransformationReturnedIncorrectUrl(string transformation) =>
            Format(section => section?.LogMessages?.TransformationReturnedIncorrectUrl, new { transformation });

        public string GetLogLineAttempt(int current, int total) =>
            Format(section => section?.LogMessages?.Attempt,
                new { current = Formatter.ToFormattedString(current), total = Formatter.ToFormattedString(total) });

        public string GetLogLineDownloadingPage(string url) => Format(section => section?.LogMessages?.DownloadingPage, new { url });

        public string GetLogLineDownloadingFile(string url) => Format(section => section?.LogMessages?.DownloadingFile, new { url });

        public string GetLogLineStartingFileDownloadKnownFileSize(long size) =>
            Format(section => section?.LogMessages?.StartingFileDownloadKnownFileSize, new { size = Formatter.ToFormattedString(size) });

        public string GetLogLineResumingFileDownloadKnownFileSize(long remaining) =>
            Format(section => section?.LogMessages?.ResumingFileDownloadKnownFileSize, new { remaining = Formatter.ToFormattedString(remaining) });

        public string GetLogLineRedirect(string url) => Format(section => section?.LogMessages?.Redirect, new { url });

        public string GetLogLineNonSuccessfulStatusCode(string status) =>
            Format(section => section?.LogMessages?.NonSuccessfulStatusCode, new { status });

        public string GetLogLineCannotCreateDownloadDirectory(string directory) =>
            Format(section => section?.LogMessages?.CannotCreateDownloadDirectory, new { directory });

        public string GetLogLineCannotCreateOrOpenFile(string file) =>
            Format(section => section?.LogMessages?.CannotCreateOrOpenFile, new { file });

        public string GetLogLineCannotRenamePartFile(string source, string destination) =>
            Format(section => section?.LogMessages?.CannotRenamePartFile, new { source, destination });

        public string GetLogLineRequestError(string url) => Format(section => section?.LogMessages?.LogLineRequestError, new { url });

        public string GetLogLineIncorrectRedirectUrl(string url) => Format(section => section?.LogMessages?.IncorrectRedirectUrl, new { url });

        public string GetLogLineUnexpectedError(string error) => Format(section => section?.LogMessages?.UnexpectedError, new { error });
    }
}

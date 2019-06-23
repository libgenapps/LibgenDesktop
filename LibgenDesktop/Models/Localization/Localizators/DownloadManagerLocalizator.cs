using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators
{
    internal class DownloadManagerLocalizator : Localizator
    {
        public DownloadManagerLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter)
        {
            TabTitle = Format(translation => translation?.TabTitle);
            Start = Format(translation => translation?.Start);
            Stop = Format(translation => translation?.Stop);
            Remove = Format(translation => translation?.Remove);
            StartAll = Format(translation => translation?.StartAll);
            StopAll = Format(translation => translation?.StopAll);
            RemoveCompleted = Format(translation => translation?.RemoveCompleted);
            QueuedStatus = Format(translation => translation?.QueuedStatus);
            DownloadingStatus = Format(translation => translation?.DownloadingStatus);
            StoppedStatus = Format(translation => translation?.StoppedStatus);
            RetryDelayStatus = Format(translation => translation?.RetryDelayStatus);
            ErrorStatus = Format(translation => translation?.ErrorStatus);
            Log = Format(translation => translation?.Log);
            TechnicalDetails = Format(translation => translation?.TechnicalDetails);
            Copy = Format(translation => translation?.Copy);
            FileNotFoundErrorTitle = Format(translation => translation?.FileNotFoundErrorTitle);
            LogLineQueued = Format(translation => translation?.LogMessages?.Queued);
            LogLineStarted = Format(translation => translation?.LogMessages?.Started);
            LogLineStopped = Format(translation => translation?.LogMessages?.Stopped);
            LogLineCompleted = Format(translation => translation?.LogMessages?.Completed);
            LogLineOfflineModeIsOn = Format(translation => translation?.LogMessages?.OfflineModeIsOn);
            LogLineMaximumDownloadAttempts = Format(translation => translation?.LogMessages?.MaximumDownloadAttempts);
            LogLineStartingFileDownloadUnknownFileSize = Format(translation => translation?.LogMessages?.StartingFileDownloadUnknownFileSize);
            LogLineResumingFileDownloadUnknownFileSize = Format(translation => translation?.LogMessages?.ResumingFileDownloadUnknownFileSize);
            LogLineRequest = Format(translation => translation?.LogMessages?.Request);
            LogLineResponse = Format(translation => translation?.LogMessages?.Response);
            LogLineTooManyRedirects = Format(translation => translation?.LogMessages?.TooManyRedirects);
            LogLineHtmlPageReturned = Format(translation => translation?.LogMessages?.HtmlPageReturned);
            LogLineNoPartialDownloadSupport = Format(translation => translation?.LogMessages?.NoPartialDownloadSupport);
            LogLineNoContentLengthWarning = Format(translation => translation?.LogMessages?.NoContentLengthWarning);
            LogLineServerResponseTimeout = Format(translation => translation?.LogMessages?.ServerResponseTimeout);
            LogLineDownloadIncompleteError = Format(translation => translation?.LogMessages?.DownloadIncompleteError);
            LogLineFileWriteError = Format(translation => translation?.LogMessages?.FileWriteError);
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
            Format(translation => translation?.DownloadProgressKnownFileSize,
                new { downloaded = Formatter.ToFormattedString(downloaded), total = Formatter.ToFormattedString(total), percent });
        public string GetDownloadProgressUnknownFileSize(long downloaded) =>
            Format(translation => translation?.DownloadProgressUnknownFileSize, new { downloaded = Formatter.ToFormattedString(downloaded) });
        public string GetFileNotFoundErrorText(string file) => Format(translation => translation?.FileNotFoundErrorText, new { file });
        public string GetLogLineRetryDelay(int count) => Format(translation => translation?.LogMessages?.RetryDelay, new { count });
        public string GetLogLineTransformationError(string transformation) =>
            Format(translation => translation?.LogMessages?.TransformationError, new { transformation });
        public string GetLogLineTransformationReturnedIncorrectUrl(string transformation) =>
            Format(translation => translation?.LogMessages?.TransformationReturnedIncorrectUrl, new { transformation });
        public string GetLogLineAttempt(int current, int total) => Format(translation => translation?.LogMessages?.Attempt, new { current, total });
        public string GetLogLineDownloadingPage(string url) => Format(translation => translation?.LogMessages?.DownloadingPage, new { url });
        public string GetLogLineDownloadingFile(string url) => Format(translation => translation?.LogMessages?.DownloadingFile, new { url });
        public string GetLogLineStartingFileDownloadKnownFileSize(long size) =>
            Format(translation => translation?.LogMessages?.StartingFileDownloadKnownFileSize, new { size });
        public string GetLogLineResumingFileDownloadKnownFileSize(long remaining) =>
            Format(translation => translation?.LogMessages?.ResumingFileDownloadKnownFileSize, new { remaining });
        public string GetLogLineRedirect(string url) => Format(translation => translation?.LogMessages?.Redirect, new { url });
        public string GetLogLineNonSuccessfulStatusCode(string status) =>
            Format(translation => translation?.LogMessages?.NonSuccessfulStatusCode, new { status });
        public string GetLogLineCannotCreateDownloadDirectory(string directory) =>
            Format(translation => translation?.LogMessages?.CannotCreateDownloadDirectory, new { directory });
        public string GetLogLineCannotCreateOrOpenFile(string file) =>
            Format(translation => translation?.LogMessages?.CannotCreateOrOpenFile, new { file });
        public string GetLogLineCannotRenamePartFile(string source, string destination) =>
            Format(translation => translation?.LogMessages?.CannotRenamePartFile, new { source, destination });
        public string GetLogLineRequestError(string url) => Format(translation => translation?.LogMessages?.LogLineRequestError, new { url });
        public string GetLogLineIncorrectRedirectUrl(string url) => Format(translation => translation?.LogMessages?.IncorrectRedirectUrl, new { url });
        public string GetLogLineUnexpectedError(string error) => Format(translation => translation?.LogMessages?.UnexpectedError, new { error });

        private string Format(Func<Translation.DownloadManagerTranslation, string> field, object templateArguments = null)
        {
            return Format(translation => field(translation?.DownloadManager), templateArguments);
        }
    }
}

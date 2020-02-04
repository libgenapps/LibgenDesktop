namespace LibgenDesktop.Models.ProgressArgs
{
    internal enum ErrorTypes
    {
        ERROR_NONE,
        ERROR_DIRECTORY_ACCESS,
        ERROR_DIRECTORY_NOT_FOUND,
        ERROR_FILE_NOT_FOUND,
        ERROR_FILE_PATH_TOO_LONG,
        ERROR_FILE_ACCESS,
        ERROR_FILE_IN_USE,
        ERROR_FILE_SIZE_ZERO,
        ERROR_IO_EXCEPTION,
        ERROR_MD5_HASH_NOT_IN_DB,
        ERROR_OTHER
    }

    internal class ScanUnknownProgress
    {
        public ScanUnknownProgress(string relativeFilePath, bool error, ErrorTypes errorType)
        {
            RelativeFilePath = relativeFilePath;
            Error = error;
            ErrorType = errorType;
        }

        public string RelativeFilePath { get; }
        public bool Error { get; }
        public ErrorTypes ErrorType {get;}
    }
}
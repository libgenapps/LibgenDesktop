using System;

namespace LibgenDesktop.Models.Utils
{
    internal static class ExceptionUtils
    {
        public static Exception GetInnermostException(this Exception exception)
        {
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
            }
            return exception;
        }
    }
}

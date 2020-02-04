using System;
using System.Runtime.InteropServices;

namespace LibgenDesktop.Models.Utils
{
    internal static class ExceptionUtils
    {
        public static int GetHRForException(Exception exception)
        {
            if (exception == null) throw new ArgumentNullException();

            //on first call there is possible pollution of thread IErrorInfo with sensitive data
            int hr = Marshal.GetHRForException(exception);
            //therefore call with empty ex. obj. to cleanup IErrorInfo
            Marshal.GetHRForException(new Exception());
            return hr;
        }

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

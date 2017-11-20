using System;
using System.Collections.Generic;
using LibgenDesktop.ViewModels;
using LibgenDesktop.Views;

namespace LibgenDesktop.Infrastructure
{
    internal static class RegisteredWindows
    {
        internal enum WindowKey
        {
            MAIN_WINDOW = 1,
            BOOK_DETAILS_WINDOW,
            ERROR_WINDOW,
            SQL_DUMP_IMPORT_WINDOW
        }

        internal class RegisteredWindow
        {
            public RegisteredWindow(WindowKey windowKey, Type windowType, Type viewModelType)
            {
                WindowKey = windowKey;
                WindowType = windowType;
                ViewModelType = viewModelType;
            }

            public WindowKey WindowKey { get; }
            public Type WindowType { get; }
            public Type ViewModelType { get; }
        }

        static RegisteredWindows()
        {
            AllWindows = new Dictionary<WindowKey, RegisteredWindow>
            {
                { WindowKey.MAIN_WINDOW, new RegisteredWindow(WindowKey.MAIN_WINDOW, typeof(MainWindow), typeof(MainWindowViewModel)) },
                { WindowKey.BOOK_DETAILS_WINDOW, new RegisteredWindow(WindowKey.BOOK_DETAILS_WINDOW, typeof(BookDetailsWindow), typeof(BookDetailsWindowViewModel)) },
                { WindowKey.ERROR_WINDOW, new RegisteredWindow(WindowKey.ERROR_WINDOW, typeof(ErrorWindow), typeof(ErrorWindowViewModel)) },
                { WindowKey.SQL_DUMP_IMPORT_WINDOW, new RegisteredWindow(WindowKey.SQL_DUMP_IMPORT_WINDOW, typeof(SqlDumpImportWindow), typeof(SqlDumpImportWindowViewModel)) }
            };
        }

        public static Dictionary<WindowKey, RegisteredWindow> AllWindows { get; }
    }
}

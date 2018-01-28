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
            NON_FICTION_DETAILS_WINDOW,
            FICTION_DETAILS_WINDOW,
            SCI_MAG_DETAILS_WINDOW,
            ERROR_WINDOW,
            IMPORT_WINDOW,
            CREATE_DATABASE_WINDOW,
            SETTINGS_WINDOW,
            SYNCHRONIZATION_WINDOW,
            APPLICATION_UPDATE_WINDOW
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
            AllWindows = new Dictionary<WindowKey, RegisteredWindow>();
            RegisterWindow(WindowKey.MAIN_WINDOW, typeof(MainWindow), typeof(MainWindowViewModel));
            RegisterWindow(WindowKey.NON_FICTION_DETAILS_WINDOW, typeof(NonFictionDetailsWindow), typeof(NonFictionDetailsWindowViewModel));
            RegisterWindow(WindowKey.FICTION_DETAILS_WINDOW, typeof(FictionDetailsWindow), typeof(FictionDetailsWindowViewModel));
            RegisterWindow(WindowKey.SCI_MAG_DETAILS_WINDOW, typeof(SciMagDetailsWindow), typeof(SciMagDetailsWindowViewModel));
            RegisterWindow(WindowKey.ERROR_WINDOW, typeof(ErrorWindow), typeof(ErrorWindowViewModel));
            RegisterWindow(WindowKey.IMPORT_WINDOW, typeof(ImportWindow), typeof(ImportWindowViewModel));
            RegisterWindow(WindowKey.CREATE_DATABASE_WINDOW, typeof(CreateDatabaseWindow), typeof(CreateDatabaseWindowViewModel));
            RegisterWindow(WindowKey.SETTINGS_WINDOW, typeof(SettingsWindow), typeof(SettingsWindowViewModel));
            RegisterWindow(WindowKey.SYNCHRONIZATION_WINDOW, typeof(SynchronizationWindow), typeof(SynchronizationWindowViewModel));
            RegisterWindow(WindowKey.APPLICATION_UPDATE_WINDOW, typeof(ApplicationUpdateWindow), typeof(ApplicationUpdateWindowViewModel));
        }

        public static Dictionary<WindowKey, RegisteredWindow> AllWindows { get; }

        private static void RegisterWindow(WindowKey windowKey, Type windowType, Type viewModelType)
        {
            AllWindows.Add(windowKey, new RegisteredWindow(windowKey, windowType, viewModelType));
        }
    }
}

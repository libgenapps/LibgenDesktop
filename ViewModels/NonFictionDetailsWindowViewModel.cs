using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Media.Imaging;
using LibgenDesktop.Common;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Settings;

namespace LibgenDesktop.ViewModels
{
    internal class NonFictionDetailsWindowViewModel : ViewModel
    {
        private readonly MainModel mainModel;
        private NonFictionBook book;
        private bool bookCoverNotLoadedDueToOfflineMode;
        private bool bookCoverLoading;
        private bool bookCoverLoadingFailed;
        private bool noCover;
        private bool bookCoverVisible;
        private BitmapImage bookCover;

        public NonFictionDetailsWindowViewModel(MainModel mainModel, NonFictionBook book)
        {
            this.mainModel = mainModel;
            this.book = book;
            DownloadBookCommand = new Command(DownloadBook);
            CloseCommand = new Command(CloseWindow);
            WindowClosedCommand = new Command(WindowClosed);
            Initialize();
        }

        public string WindowTitle { get; private set; }
        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }
        public bool IsInOfflineMode { get; private set; }

        public NonFictionBook Book
        {
            get
            {
                return book;
            }
            private set
            {
                book = value;
                NotifyPropertyChanged();
            }
        }

        public bool BookCoverNotLoadedDueToOfflineMode
        {
            get
            {
                return bookCoverNotLoadedDueToOfflineMode;
            }
            private set
            {
                bookCoverNotLoadedDueToOfflineMode = value;
                NotifyPropertyChanged();
            }
        }

        public bool BookCoverLoading
        {
            get
            {
                return bookCoverLoading;
            }
            private set
            {
                bookCoverLoading = value;
                NotifyPropertyChanged();
            }
        }

        public bool BookCoverLoadingFailed
        {
            get
            {
                return bookCoverLoadingFailed;
            }
            private set
            {
                bookCoverLoadingFailed = value;
                NotifyPropertyChanged();
            }
        }

        public bool NoCover
        {
            get
            {
                return noCover;
            }
            private set
            {
                noCover = value;
                NotifyPropertyChanged();
            }
        }

        public bool BookCoverVisible
        {
            get
            {
                return bookCoverVisible;
            }
            private set
            {
                bookCoverVisible = value;
                NotifyPropertyChanged();
            }
        }

        public BitmapImage BookCover
        {
            get
            {
                return bookCover;
            }
            private set
            {
                bookCover = value;
                NotifyPropertyChanged();
            }
        }

        public Command DownloadBookCommand { get; }
        public Command CloseCommand { get; }
        public Command WindowClosedCommand { get; }

        private async void Initialize()
        {
            WindowTitle = Book.Title;
            WindowWidth = mainModel.AppSettings.NonFiction.DetailsWindow.Width;
            WindowHeight = mainModel.AppSettings.NonFiction.DetailsWindow.Height;
            if (mainModel.AppSettings.Network.OfflineMode)
            {
                IsInOfflineMode = true;
                BookCoverNotLoadedDueToOfflineMode = true;
                BookCoverLoading = false;
            }
            else
            {
                IsInOfflineMode = false;
                BookCoverNotLoadedDueToOfflineMode = false;
                BookCoverLoading = true;
            }
            BookCoverLoadingFailed = false;
            NoCover = false;
            BookCoverVisible = false;
            BookCover = null;
            if (String.IsNullOrWhiteSpace(Book.CoverUrl))
            {
                BookCoverNotLoadedDueToOfflineMode = false;
                BookCoverLoading = false;
                NoCover = true;
            }
            else
            {
                if (!IsInOfflineMode)
                {
                    try
                    {
                        WebClient webClient = new WebClient();
                        byte[] imageData = await webClient.DownloadDataTaskAsync(new Uri(Constants.NON_FICTION_COVER_URL_PREFIX + Book.CoverUrl));
                        BitmapImage bitmapImage = new BitmapImage();
                        using (MemoryStream memoryStream = new MemoryStream(imageData))
                        {
                            bitmapImage.BeginInit();
                            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                            bitmapImage.StreamSource = memoryStream;
                            bitmapImage.EndInit();
                            bitmapImage.Freeze();
                        }
                        BookCover = bitmapImage;
                        BookCoverVisible = true;
                    }
                    catch
                    {
                        BookCoverLoadingFailed = true;
                    }
                    BookCoverLoading = false;
                }
            }
        }

        private void DownloadBook()
        {
            Process.Start(Constants.NON_FICTION_DOWNLOAD_URL_PREFIX + Book.Md5Hash);
        }

        private void CloseWindow()
        {
            IWindowContext currentWindowContext = WindowManager.GetWindowContext(this);
            currentWindowContext.CloseDialog(false);
        }

        private void WindowClosed()
        {
            mainModel.AppSettings.NonFiction.DetailsWindow = new AppSettings.NonFictionDetailsWindowSettings
            {
                Width = WindowWidth,
                Height = WindowHeight
            };
            mainModel.SaveSettings();
        }
    }
}

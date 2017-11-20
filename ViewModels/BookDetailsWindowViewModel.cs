using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using LibgenDesktop.Common;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Settings;

namespace LibgenDesktop.ViewModels
{
    internal class BookDetailsWindowViewModel : ViewModel
    {
        private readonly MainModel mainModel;
        private bool bookCoverNotLoadedDueToOfflineMode;
        private bool bookCoverLoading;
        private bool bookCoverLoadingFailed;
        private bool noCover;
        private bool bookCoverVisible;
        private BitmapImage bookCover;

        public BookDetailsWindowViewModel(MainModel mainModel, Book mainBookInfo)
        {
            this.mainModel = mainModel;
            DownloadBookCommand = new Command(DownloadBook);
            CloseCommand = new Command(CloseWindow);
            WindowClosedCommand = new Command(WindowClosed);
            Initialize(mainBookInfo);
        }

        public Book Book { get; private set; }
        public string WindowTitle { get; private set; }
        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }
        public bool IsInOfflineMode { get; private set; }

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

        private async void Initialize(Book mainBookInfo)
        {
            WindowTitle = mainBookInfo.Title;
            WindowWidth = mainModel.AppSettings.BookWindow.Width;
            WindowHeight = mainModel.AppSettings.BookWindow.Height;
            if (mainModel.AppSettings.OfflineMode)
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
            Book = await mainModel.LoadBookAsync(mainBookInfo.Id);
            NotifyPropertyChanged(nameof(Book));
            if (String.IsNullOrWhiteSpace(Book.ExtendedProperties.CoverUrl))
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
                        byte[] imageData = await webClient.DownloadDataTaskAsync(new Uri(Constants.BOOK_COVER_URL_PREFIX + Book.ExtendedProperties.CoverUrl));
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
            Process.Start(Constants.BOOK_DOWNLOAD_URL_PREFIX + Book.ExtendedProperties.Md5Hash);
        }

        private void CloseWindow()
        {
            IWindowContext currentWindowContext = WindowManager.GetCreatedWindowContext(this);
            currentWindowContext.CloseDialog(false);
        }

        private void WindowClosed()
        {
            mainModel.AppSettings.BookWindow = new AppSettings.BookWindowSettings
            {
                Width = WindowWidth,
                Height = WindowHeight
            };
            mainModel.SaveSettings();
        }
    }
}

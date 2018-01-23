using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Utils;

namespace LibgenDesktop.ViewModels
{
    internal class NonFictionDetailsTabViewModel : TabViewModel
    {
        private NonFictionBook book;
        private bool isBookCoverNotificationVisible;
        private string bookCoverNotification;
        private bool bookCoverVisible;
        private BitmapImage bookCover;
        private string downloadButtonCaption;
        private bool isDownloadButtonEnabled;
        private string disabledDownloadButtonTooltip;
        private string bookDownloadUrl;

        public NonFictionDetailsTabViewModel(MainModel mainModel, IWindowContext parentWindowContext, NonFictionBook book, bool isInModalWindow)
            : base(mainModel, parentWindowContext, book.Title)
        {
            this.book = book;
            IsInModalWindow = isInModalWindow;
            DownloadBookCommand = new Command(DownloadBook);
            CloseCommand = new Command(CloseTab);
            Initialize();
        }

        public bool IsInModalWindow { get; }

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

        public bool IsBookCoverNotificationVisible
        {
            get
            {
                return isBookCoverNotificationVisible;
            }
            set
            {
                isBookCoverNotificationVisible = value;
                NotifyPropertyChanged();
            }
        }

        public string BookCoverNotification
        {
            get
            {
                return bookCoverNotification;
            }
            set
            {
                bookCoverNotification = value;
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

        public string DownloadButtonCaption
        {
            get
            {
                return downloadButtonCaption;
            }
            set
            {
                downloadButtonCaption = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsDownloadButtonEnabled
        {
            get
            {
                return isDownloadButtonEnabled;
            }
            set
            {
                isDownloadButtonEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public string DisabledDownloadButtonTooltip
        {
            get
            {
                return disabledDownloadButtonTooltip;
            }
            set
            {
                disabledDownloadButtonTooltip = value;
                NotifyPropertyChanged();
            }
        }

        public Command DownloadBookCommand { get; }
        public Command CloseCommand { get; }

        public event EventHandler CloseTabRequested;

        private async void Initialize()
        {
            bool isInOfflineMode = MainModel.AppSettings.Network.OfflineMode;
            string downloadMirrorName = MainModel.AppSettings.Mirrors.NonFictionBooksMirrorName;
            string coverMirrorName = MainModel.AppSettings.Mirrors.NonFictionCoversMirrorName;
            if (downloadMirrorName == null)
            {
                DownloadButtonCaption = "СКАЧАТЬ";
                IsDownloadButtonEnabled = false;
                DisabledDownloadButtonTooltip = "Не выбрано зеркало для загрузки книг";
                bookDownloadUrl = null;
            }
            else
            {
                DownloadButtonCaption = "СКАЧАТЬ С " + downloadMirrorName.ToUpper();
                if (isInOfflineMode)
                {
                    IsDownloadButtonEnabled = false;
                    DisabledDownloadButtonTooltip = "Включен автономный режим";
                    bookDownloadUrl = null;
                }
                else
                {
                    IsDownloadButtonEnabled = true;
                    bookDownloadUrl = Book.Env(MainModel.Mirrors[downloadMirrorName].NonFictionDownloadUrl);
                }
            }
            BookCoverVisible = false;
            BookCover = null;
            bool hasCover = !String.IsNullOrWhiteSpace(Book.CoverUrl);
            if (!hasCover)
            {
                BookCoverNotification = "Обложка отсутствует";
                IsBookCoverNotificationVisible = true;
            }
            else
            {
                if (coverMirrorName == null)
                {
                    BookCoverNotification = "Не выбрано зеркало\r\nдля загрузки обложек";
                    IsBookCoverNotificationVisible = true;
                }
                else
                {
                    if (isInOfflineMode)
                    {
                        BookCoverNotification = "Обложка не загружена, потому что\r\nвключен автономный режим";
                        IsBookCoverNotificationVisible = true;
                    }
                    else
                    {
                        string bookCoverUrl = Book.Env(MainModel.Mirrors[coverMirrorName].NonFictionCoverUrl);
                        BookCoverNotification = "Обложка загружается...";
                        IsBookCoverNotificationVisible = true;
                        try
                        {
                            byte[] imageData = await MainModel.HttpClient.GetByteArrayAsync(bookCoverUrl);
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
                            IsBookCoverNotificationVisible = false;
                        }
                        catch
                        {
                            BookCoverNotification = "Не удалось загрузить обложку";
                        }
                    }
                }
            }
        }

        private void DownloadBook()
        {
            Process.Start(bookDownloadUrl);
        }

        private void CloseTab()
        {
            CloseTabRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}

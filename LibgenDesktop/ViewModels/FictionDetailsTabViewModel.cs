using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Media.Imaging;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Entities;

namespace LibgenDesktop.ViewModels
{
    internal class FictionDetailsTabViewModel : TabViewModel
    {
        private FictionBook book;
        private bool isBookCoverNotificationVisible;
        private string bookCoverNotification;
        private bool bookCoverVisible;
        private BitmapImage bookCover;
        private string downloadButtonCaption;
        private bool isDownloadButtonEnabled;
        private string disabledDownloadButtonTooltip;
        private string bookDownloadUrl;

        public FictionDetailsTabViewModel(MainModel mainModel, FictionBook book, bool isInModalWindow)
            : base(mainModel, book.Title)
        {
            this.book = book;
            IsInModalWindow = isInModalWindow;
            DownloadBookCommand = new Command(DownloadBook);
            CloseCommand = new Command(CloseTab);
            Initialize();
        }

        public bool IsInModalWindow { get; }

        public FictionBook Book
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
            DownloadButtonCaption = "СКАЧАТЬ С " + MainModel.AppSettings.Network.MirrorName.ToUpper();
            bookDownloadUrl = null;
            BookCoverVisible = false;
            BookCover = null;
            bool hasCover = Book.Cover == "1";
            if (!hasCover)
            {
                BookCoverNotification = "Обложка отсутствует";
                IsBookCoverNotificationVisible = true;
            }
            if (MainModel.AppSettings.Network.OfflineMode)
            {
                IsDownloadButtonEnabled = false;
                DisabledDownloadButtonTooltip = "Включен автономный режим";
                if (hasCover)
                {
                    BookCoverNotification = "Обложка не загружена, потому что\r\nвключен автономный режим";
                    IsBookCoverNotificationVisible = true;
                }
            }
            else
            {
                if (!String.IsNullOrWhiteSpace(MainModel.CurrentMirror.FictionDownloadUrl))
                {
                    IsDownloadButtonEnabled = true;
                    bookDownloadUrl = MainModel.CurrentMirror.FictionDownloadUrl + Book.Md5Hash;
                }
                else
                {
                    IsDownloadButtonEnabled = false;
                    DisabledDownloadButtonTooltip = "Выбранное зеркало не поддерживает загрузку художественной литературы";
                }
                if (hasCover)
                {
                    if (!String.IsNullOrWhiteSpace(MainModel.CurrentMirror.FictionCoverUrl))
                    {
                        BookCoverNotification = "Обложка загружается...";
                        IsBookCoverNotificationVisible = true;
                        try
                        {
                            string bookCoverUrl = String.Concat(Book.LibgenId / 1000 * 1000, "/", Book.Md5Hash, ".jpg");
                            WebClient webClient = new WebClient();
                            byte[] imageData = await webClient.DownloadDataTaskAsync(new Uri(MainModel.CurrentMirror.FictionCoverUrl + bookCoverUrl));
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
                    else
                    {
                        BookCoverNotification = "Выбранное зеркало не поддерживает\r\nзагрузку обложек для\r\nхудожественной литературы";
                        IsBookCoverNotificationVisible = true;
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

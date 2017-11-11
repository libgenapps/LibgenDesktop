using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using LibgenDesktop.Database;
using LibgenDesktop.Settings;

namespace LibgenDesktop.Interface
{
    public partial class BookForm : Form
    {
        internal class BookProperty
        {
            public string Title { get; set; }
            public string Value { get; set; }
        }

        private const string BOOK_COVER_URL_PREFIX = "http://libgen.io/covers/";
        private const string BOOK_DOWNLOAD_URL_PREFIX = "http://libgen.io/book/index.php?md5=";

        private readonly Book book;
        private readonly bool offlineMode;
        private List<BookProperty> bookProperties;
        private ToolTip offlineModeTooltip;
        private bool offlineModeTooltipVisible;

        internal BookForm(Book book)
        {
            InitializeComponent();
            this.book = book;
            AppSettings appSettings = SettingsStorage.AppSettings;
            offlineMode = appSettings.OfflineMode;
            Width = appSettings.BookWindow.Width;
            Height = appSettings.BookWindow.Height;
            MinimumSize = new Size(AppSettings.BOOK_WINDOW_MIN_WIDTH, AppSettings.BOOK_WINDOW_MIN_HEIGHT);
            Text = $"{book.Title} (книга №{book.Id})";
            PopulateBookFields();
            LoadCover(book.ExtendedProperties.CoverUrl);
        }

        private void BookForm_Load(object sender, EventArgs e)
        {
            Icon = IconUtils.GetAppIcon();
            if (offlineMode)
            {
                downloadButton.Enabled = false;
                offlineModeTooltip = new ToolTip();
                offlineModeTooltipVisible = false;
            }
        }

        private void PopulateBookFields()
        {
            bookProperties = new List<BookProperty>();
            AddBookProperty("Наименование", book.Title);
            AddBookProperty("Авторы", book.Authors);
            AddBookProperty("Серия", book.Series);
            AddBookProperty("Издатель", book.Title);
            AddBookProperty("Год", book.Year);
            AddBookProperty("Язык", book.ExtendedProperties.Language);
            AddBookProperty("Формат", book.Format);
            AddBookProperty("ISBN", book.ExtendedProperties.Identifier);
            AddBookProperty("Добавлено", book.ExtendedProperties.AddedDateTime.ToString("dd.MM.yyyy HH:mm:ss"));
            AddBookProperty("Обновлено", book.ExtendedProperties.LastModifiedDateTime.ToString("dd.MM.yyyy HH:mm:ss"));
            AddBookProperty("Библиотека", book.ExtendedProperties.Library);
            AddBookProperty("Размер файла", book.FileSizeWithBytesString);
            AddBookProperty("Темы", book.ExtendedProperties.Topic);
            AddBookProperty("Том", book.ExtendedProperties.VolumeInfo);
            AddBookProperty("Журнал", book.ExtendedProperties.Periodical);
            AddBookProperty("Город", book.ExtendedProperties.City);
            AddBookProperty("Издание", book.ExtendedProperties.Edition);
            AddBookProperty("Страниц", PagesToString(book.ExtendedProperties.Pages, book.ExtendedProperties.PagesInFile));
            AddBookProperty("Теги", book.ExtendedProperties.Tags);
            AddBookProperty("MD5-хэш", book.ExtendedProperties.Md5Hash);
            AddBookProperty("Libgen ID", book.ExtendedProperties.LibgenId.ToString());
            AddBookProperty("ISSN", book.ExtendedProperties.Issn);
            AddBookProperty("UDC", book.ExtendedProperties.Udc);
            AddBookProperty("LBC", book.ExtendedProperties.Lbc);
            AddBookProperty("LCC", book.ExtendedProperties.Lcc);
            AddBookProperty("DDC", book.ExtendedProperties.Ddc);
            AddBookProperty("DOI", book.ExtendedProperties.Doi);
            AddBookProperty("OpenLibraryID", book.ExtendedProperties.Doi);
            AddBookProperty("GoogleID", book.ExtendedProperties.Doi);
            AddBookProperty("ASIN", book.ExtendedProperties.Doi);
            AddBookProperty("DPI", book.ExtendedProperties.Dpi.ToString());
            AddBookProperty("OCR", StringBooleanToLabelString(book.Searchable, "да", "нет", "неизвестно"));
            AddBookProperty("Оглавление", StringBooleanToLabelString(book.ExtendedProperties.Bookmarked, "есть", "нет", "неизвестно"));
            AddBookProperty("Отсканирована", StringBooleanToLabelString(book.ExtendedProperties.Scanned, "да", "нет", "неизвестно"));
            AddBookProperty("Ориентация", StringBooleanToLabelString(book.ExtendedProperties.Orientation, "портретная", "альбомная", "неизвестно"));
            AddBookProperty("Постраничная", StringBooleanToLabelString(book.ExtendedProperties.Paginated, "есть", "нет", "неизвестно"));
            AddBookProperty("Цветная", StringBooleanToLabelString(book.ExtendedProperties.Color, "да", "нет", "неизвестно"));
            AddBookProperty("Вычищенная", StringBooleanToLabelString(book.ExtendedProperties.Cleaned, "да", "нет", "неизвестно"));
            AddBookProperty("Комментарий", book.ExtendedProperties.Commentary);
            bookDetailsDataGridView.DataSource = bookProperties;
        }

        private void AddBookProperty(string title, string value)
        {
            bookProperties.Add(new BookProperty
            {
                Title = title + ':',
                Value = value
            });
        }

        private void ValueLabel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                copyValueLabelTextMenuItem.Text = $"Скопировать \"{(sender as Label).Text}\"";
            }
        }

        private string StringBooleanToLabelString(string value, string value1Label, string value0Label, string valueUnknownLabel)
        {
            switch (value)
            {
                case "0":
                    return value0Label;
                case "1":
                    return value1Label;
                default:
                    return valueUnknownLabel;
            }
        }

        private string PagesToString(string pages, int pagesInFile)
        {
            StringBuilder resultBuilder = new StringBuilder();
            if (!String.IsNullOrWhiteSpace(pages))
            {
                resultBuilder.Append(pages);
            }
            else
            {
                resultBuilder.Append("неизвестно");
            }
            resultBuilder.Append(" (содержательная часть) / ");
            resultBuilder.Append(pagesInFile.ToString());
            resultBuilder.Append(" (всего в файле)");
            return resultBuilder.ToString();
        }

        private void LoadCover(string bookCoverUrl)
        {
            if (String.IsNullOrWhiteSpace(bookCoverUrl))
            {
                coverLoadingLabel.Text = "Обложка отсутствует";
            }
            else
            {
                if (offlineMode)
                {
                    coverLoadingLabel.Text = "Обложка не загружена, потому что\r\nвключен автономный режим";
                }
                else
                {
                    coverLoadingLabel.Text = "Загружается обложка...";
                    WebClient webClient = new WebClient();
                    webClient.DownloadDataCompleted += WebClient_DownloadDataCompleted;
                    webClient.DownloadDataAsync(new Uri(BOOK_COVER_URL_PREFIX + bookCoverUrl));
                }
            }
        }

        private void WebClient_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                coverLoadingLabel.Text = "Не удалось загрузить обложку";
            }
            else
            {
                coverLoadingLabel.Visible = false;
                using (MemoryStream memoryStream = new MemoryStream(e.Result))
                using (Image image = Image.FromStream(memoryStream))
                {
                    bookCoverPictureBox.Image = ResizeImage(image);
                }
            }
        }

        private Image ResizeImage(Image source)
        {
            int width = bookCoverPictureBox.Width;
            int height = (int)Math.Round(source.Height * ((double)width / source.Width));
            using (Bitmap bitmap = new Bitmap(width, height))
            using (Graphics graphics = Graphics.FromImage((Image)bitmap))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(source, 0, 0, width, height);
                return (Image)bitmap.Clone();
            }
        }

        private void downloadButton_Click(object sender, EventArgs e)
        {
            Process.Start(BOOK_DOWNLOAD_URL_PREFIX + book.ExtendedProperties.Md5Hash);
        }

        private void BookForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (offlineModeTooltip == null)
            {
                return;
            }
            Control controlUnderPointer = GetChildAtPoint(e.Location);
            if (controlUnderPointer == downloadButton)
            {
                if (!offlineModeTooltipVisible)
                {
                    offlineModeTooltip.Show("Включен автономный режим", downloadButton, downloadButton.Width / 2, downloadButton.Height);
                    offlineModeTooltipVisible = true;
                }
            }
            else
            {
                if (offlineModeTooltipVisible)
                {
                    offlineModeTooltip.Hide(downloadButton);
                    offlineModeTooltipVisible = false;
                }
            }
        }

        private void BookForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            SettingsStorage.AppSettings.BookWindow.Width = Width;
            SettingsStorage.AppSettings.BookWindow.Height = Height;
            SettingsStorage.SaveSettings();
        }

        private void copyValueLabelTextMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText((sender as ToolStripMenuItem).Tag.ToString());
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            bookDetailsDataGridView.ClearSelection();
        }

        private void bookDetailsDataGridView_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                string cellText = bookDetailsDataGridView.Rows[e.RowIndex].Cells[1].Value.ToString();
                if (!String.IsNullOrWhiteSpace(cellText))
                {
                    copyValueLabelTextMenuItem.Text = $"Скопировать \"{cellText}\"";
                    copyValueLabelTextMenuItem.Tag = cellText;
                    e.ContextMenuStrip = valueLabelContextMenu;
                }
            }
        }
    }
}

using System;
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
        private const string BOOK_COVER_URL_PREFIX = "http://libgen.io/covers/";
        private const string BOOK_DOWNLOAD_URL_PREFIX = "http://libgen.io/book/index.php?md5=";

        private readonly Book book;
        private readonly bool offlineMode;
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
            PopulateBookFields();
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
            Text = $"{book.Title} (книга №{book.Id})";
            SetValueLabelText(titleValueLabel, book.Title);
            SetValueLabelText(authorsValueLabel, book.Authors);
            SetValueLabelText(seriesValueLabel, book.Series);
            SetValueLabelText(publisherValueLabel, book.Publisher);
            SetValueLabelText(yearValueLabel, book.Year);
            SetValueLabelText(languageValueLabel, book.ExtendedProperties.Language);
            SetValueLabelText(formatValueLabel, book.Format);
            SetValueLabelText(identifierValueLabel, book.ExtendedProperties.Identifier);
            SetValueLabelText(addedValueLabel, book.ExtendedProperties.AddedDateTime.ToString("dd.MM.yyyy HH:mm:ss"));
            SetValueLabelText(updatedValueLabel, book.ExtendedProperties.LastModifiedDateTime.ToString("dd.MM.yyyy HH:mm:ss"));
            SetValueLabelText(libraryValueLabel, book.ExtendedProperties.Library);
            SetValueLabelText(fileSizeValueLabel, book.FileSizeWithBytesString);
            SetValueLabelText(commentValueLabel, book.ExtendedProperties.Commentary);
            SetValueLabelText(topicsValueLabel, book.ExtendedProperties.Topic);
            SetValueLabelText(volumeValueLabel, book.ExtendedProperties.VolumeInfo);
            SetValueLabelText(periodicalValueLabel, book.ExtendedProperties.Periodical);
            SetValueLabelText(cityValueLabel, book.ExtendedProperties.City);
            SetValueLabelText(editionValueLabel, book.ExtendedProperties.Edition);
            SetValueLabelText(pagesValueLabel, PagesToString(book.ExtendedProperties.Pages, book.ExtendedProperties.PagesInFile));
            SetValueLabelText(tagsValueLabel, book.ExtendedProperties.Tags);
            SetValueLabelText(md5HashValueLabel, book.ExtendedProperties.Md5Hash);
            SetValueLabelText(libgenIdValueLabel, book.ExtendedProperties.LibgenId.ToString());
            SetValueLabelText(issnValueLabel, book.ExtendedProperties.Issn);
            SetValueLabelText(udcValueLabel, book.ExtendedProperties.Udc);
            SetValueLabelText(lbcValueLabel, book.ExtendedProperties.Lbc);
            SetValueLabelText(lccValueLabel, book.ExtendedProperties.Lcc);
            SetValueLabelText(ddcValueLabel, book.ExtendedProperties.Ddc);
            SetValueLabelText(doiValueLabel, book.ExtendedProperties.Doi);
            SetValueLabelText(openLibraryIdValueLabel, book.ExtendedProperties.OpenLibraryId);
            SetValueLabelText(googleIdValueLabel, book.ExtendedProperties.GoogleBookid);
            SetValueLabelText(asinValueLabel, book.ExtendedProperties.Asin);
            SetValueLabelText(dpiValueLabel, book.ExtendedProperties.Dpi.ToString());
            ocrValueLabel.Text = StringBooleanToLabelString(book.ExtendedProperties.Searchable, "да", "нет", "неизвестно");
            bookmarkedValueLabel.Text = StringBooleanToLabelString(book.ExtendedProperties.Bookmarked, "есть", "нет", "неизвестно");
            scannedValueLabel.Text = StringBooleanToLabelString(book.ExtendedProperties.Scanned, "да", "нет", "неизвестно");
            orientationValueLabel.Text = StringBooleanToLabelString(book.ExtendedProperties.Orientation, "портретная", "альбомная", "неизвестно");
            paginatedValueLabel.Text = StringBooleanToLabelString(book.ExtendedProperties.Paginated, "есть", "нет", "неизвестно");
            colorValueLabel.Text = StringBooleanToLabelString(book.ExtendedProperties.Color, "да", "нет", "неизвестно");
            cleanedValueLabel.Text = StringBooleanToLabelString(book.ExtendedProperties.Cleaned, "да", "нет", "неизвестно");
            LoadCover(book.ExtendedProperties.CoverUrl);
        }

        private void SetValueLabelText(Label valueLabel, string text)
        {
            valueLabel.Text = text;
            if (!String.IsNullOrWhiteSpace(text))
            {
                valueLabel.ContextMenuStrip = valueLabelContextMenu;
                valueLabel.MouseDown += ValueLabel_MouseDown;
            }
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
            foreach (Label label in Controls.OfType<Label>())
            {
                label.MouseDown -= ValueLabel_MouseDown;
            }
            SettingsStorage.AppSettings.BookWindow.Width = Width;
            SettingsStorage.AppSettings.BookWindow.Height = Height;
            SettingsStorage.SaveSettings();
        }

        private void copyValueLabelTextMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(((sender as ToolStripMenuItem).Owner as ContextMenuStrip).SourceControl.Text);
        }
    }
}

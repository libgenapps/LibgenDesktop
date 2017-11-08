using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using LibgenDesktop.Database;

namespace LibgenDesktop.Interface
{
    public partial class BookForm : Form
    {
        private const string BOOK_COVER_URL_PREFIX = "http://libgen.io/covers/";
        private const string BOOK_DOWNLOAD_URL_PREFIX = "http://libgen.io/book/index.php?md5=";

        private readonly Book book;

        internal BookForm(Book book)
        {
            InitializeComponent();
            this.book = book;
            PopulateBookFields();
        }

        private void PopulateBookFields()
        {
            Text = $"{book.Title} (книга №{book.Id})";
            titleValueLabel.Text = book.Title;
            authorsValueLabel.Text = book.Authors;
            seriesValueLabel.Text = book.Series;
            publisherValueLabel.Text = book.Publisher;
            languageValueLabel.Text = book.ExtendedProperties.Language;
            formatValueLabel.Text = book.Format;
            identifierValueLabel.Text = book.ExtendedProperties.Identifier;
            addedValueLabel.Text = book.ExtendedProperties.AddedDateTime.ToString("dd.MM.yyyy HH:mm:ss");
            updatedValueLabel.Text = book.ExtendedProperties.LastModifiedDateTime.ToString("dd.MM.yyyy HH:mm:ss");
            libraryValueLabel.Text = book.ExtendedProperties.Library;
            fileSizeValueLabel.Text = FileSizeToString(book.SizeInBytes);
            commentValueLabel.Text = book.ExtendedProperties.Commentary;
            topicsValueLabel.Text = book.ExtendedProperties.Topic;
            volumeValueLabel.Text = book.ExtendedProperties.VolumeInfo;
            periodicalValueLabel.Text = book.ExtendedProperties.Periodical;
            cityValueLabel.Text = book.ExtendedProperties.City;
            editionValueLabel.Text = book.ExtendedProperties.Edition;
            pagesValueLabel.Text = PagesToString(book.ExtendedProperties.Pages, book.ExtendedProperties.PagesInFile);
            tagsValueLabel.Text = book.ExtendedProperties.Tags;
            md5HashValueLabel.Text = book.ExtendedProperties.Md5Hash;
            libgenIdValueLabel.Text = book.ExtendedProperties.LibgenId.ToString();
            issnValueLabel.Text = book.ExtendedProperties.Issn;
            udcValueLabel.Text = book.ExtendedProperties.Udc;
            lbcValueLabel.Text = book.ExtendedProperties.Lbc;
            lccValueLabel.Text = book.ExtendedProperties.Lcc;
            ddcValueLabel.Text = book.ExtendedProperties.Ddc;
            doiValueLabel.Text = book.ExtendedProperties.Doi;
            openLibraryIdValueLabel.Text = book.ExtendedProperties.OpenLibraryId;
            googleIdValueLabel.Text = book.ExtendedProperties.GoogleBookid;
            asinValueLabel.Text = book.ExtendedProperties.Asin;
            dpiValueLabel.Text = book.ExtendedProperties.Dpi.ToString();
            ocrValueLabel.Text = StringBooleanToLabelString(book.ExtendedProperties.Searchable, "да", "нет", "неизвестно");
            bookmarkedValueLabel.Text = StringBooleanToLabelString(book.ExtendedProperties.Bookmarked, "есть", "нет", "неизвестно");
            scannedValueLabel.Text = StringBooleanToLabelString(book.ExtendedProperties.Scanned, "да", "нет", "неизвестно");
            orientationValueLabel.Text = StringBooleanToLabelString(book.ExtendedProperties.Orientation, "портретная", "альбомная", "неизвестно");
            paginatedValueLabel.Text = StringBooleanToLabelString(book.ExtendedProperties.Paginated, "есть", "нет", "неизвестно");
            colorValueLabel.Text = StringBooleanToLabelString(book.ExtendedProperties.Color, "да", "нет", "неизвестно");
            cleanedValueLabel.Text = StringBooleanToLabelString(book.ExtendedProperties.Cleaned, "да", "нет", "неизвестно");
            LoadCover(book.ExtendedProperties.CoverUrl);
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

        private string FileSizeToString(long fileSize)
        {
            string[] postfixes = new[] { "байт", "Кб", "Мб", "Гб", "Тб" };
            int postfixIndex = fileSize != 0 ? (int)Math.Floor(Math.Log(fileSize) / Math.Log(1024)) : 0;
            StringBuilder resultBuilder = new StringBuilder();
            resultBuilder.Append((fileSize / Math.Pow(1024, postfixIndex)).ToString("N2"));
            resultBuilder.Append(" ");
            resultBuilder.Append(postfixes[postfixIndex]);
            if (postfixIndex != 0)
            {
                NumberFormatInfo fileSizeFormat = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
                fileSizeFormat.NumberGroupSeparator = " ";
                resultBuilder.Append(" (");
                resultBuilder.Append(fileSize.ToString("N0", fileSizeFormat));
                resultBuilder.Append(" байт)");
            }
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
                coverLoadingLabel.Text = "Загружается обложка...";
                WebClient webClient = new WebClient();
                webClient.DownloadDataCompleted += WebClient_DownloadDataCompleted;
                webClient.DownloadDataAsync(new Uri(BOOK_COVER_URL_PREFIX + bookCoverUrl));
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
    }
}

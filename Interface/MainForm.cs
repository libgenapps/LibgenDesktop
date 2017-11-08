using System;
using System.Drawing;
using System.Windows.Forms;
using LibgenDesktop.Cache;
using LibgenDesktop.Database;
using LibgenDesktop.Infrastructure;

namespace LibgenDesktop.Interface
{
    public partial class MainForm : Form
    {
        private const int MIN_BOOK_COUNT_FOR_INITIAL_LOAD_UPDATE = 20000;

        private readonly LocalDatabase localDatabase;
        private readonly DataCache dataCache;
        private ProgressOperation currentProgressOperation;

        public MainForm()
        {
            localDatabase = new LocalDatabase();
            dataCache = new DataCache(localDatabase);
            currentProgressOperation = null;
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            mainMenu.BackColor = Color.White;
            statusPanel.BackColor = Color.White;
            StartProgressOperation(dataCache.CreateLoadBooksOperation());
            bookListView.VirtualListDataSource = dataCache.DataAccessor;
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            searchTextBox.Focus();
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void openDatabaseMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void importFromSqlDumpMenuItem_Click(object sender, EventArgs e)
        {
            if (openSqlDumpDialog.ShowDialog() == DialogResult.OK)
            {
                ProgressOperation importSqlDumpOperation = dataCache.CreateImportSqlDumpOperation(openSqlDumpDialog.FileName);
                ProgressForm progressForm = new ProgressForm(importSqlDumpOperation);
                progressForm.ShowDialog();
                searchTextBox.Clear();
                bookListView.VirtualListDataSource = dataCache.DataAccessor;
                ShowBookCountInStatusLabel();
                importFromSqlDumpMenuItem.Enabled = dataCache.DataAccessor.GetObjectCount() == 0;
            }
        }

        private void StartProgressOperation(ProgressOperation progressOperation)
        {
            currentProgressOperation = progressOperation;
            progressOperation.ProgressEvent += ProgressOperation_ProgressEvent;
            progressOperation.CompletedEvent += ProgressOperation_CompletedEvent;
            progressOperation.CancelledEvent += ProgressOperation_CancelledEvent;
            progressBar.Value = 0;
            progressBar.Visible = true;
            searchTextBox.ReadOnly = true;
            progressBar.Style = progressOperation.IsUnbounded ? ProgressBarStyle.Marquee : ProgressBarStyle.Continuous;
            progressOperation.Start();
        }

        private void ProgressOperation_ProgressEvent(object sender, ProgressEventArgs e)
        {
            BeginInvoke(new Action(() =>
            {
                bool updateBookListView = bookListView.VirtualListSize == 0;
                if (!updateBookListView)
                {
                    int newBookCount = dataCache.DataAccessor.GetObjectCount() - bookListView.VirtualListSize;
                    updateBookListView = newBookCount > MIN_BOOK_COUNT_FOR_INITIAL_LOAD_UPDATE && newBookCount > bookListView.VirtualListSize;
                }
                if (updateBookListView)
                {
                    bookListView.UpdateVirtualListSize();
                }
                if (!Double.IsNaN(e.PercentCompleted))
                {
                    progressBar.SetProgressNoAnimation((int)Math.Truncate(e.PercentCompleted * 10));
                }
                if (e.ProgressDescription != null)
                {
                    statusLabel.Text = e.ProgressDescription;
                }
            }));
        }

        private void ProgressOperation_CompletedEvent(object sender, EventArgs e)
        {
            BeginInvoke(new Action(() =>
            {
                bookListView.UpdateVirtualListSize();
                progressBar.Visible = false;
                searchTextBox.ReadOnly = false;
                ShowBookCountInStatusLabel();
                if (dataCache.IsInAllBooksMode)
                {
                    importFromSqlDumpMenuItem.Enabled = dataCache.DataAccessor.GetObjectCount() == 0;
                }
            }));
            RemoveProgressOperation();
        }

        private void ProgressOperation_CancelledEvent(object sender, EventArgs e)
        {
            BeginInvoke(new Action(() => progressBar.Visible = false));
            RemoveProgressOperation();
        }

        private void StopProgressOperation()
        {
            currentProgressOperation?.Cancel();
            RemoveProgressOperation();
        }

        private void RemoveProgressOperation()
        {
            ProgressOperation removingProgressOperation = currentProgressOperation;
            if (removingProgressOperation != null)
            {
                removingProgressOperation.ProgressEvent -= ProgressOperation_ProgressEvent;
                removingProgressOperation.CompletedEvent -= ProgressOperation_CompletedEvent;
                removingProgressOperation.CancelledEvent -= ProgressOperation_CancelledEvent;
                currentProgressOperation = null;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopProgressOperation();
        }

        private void bookListView_DoubleClick(object sender, EventArgs e)
        {
            Book selectedBook = bookListView.SelectedObject as Book;
            selectedBook = localDatabase.GetBookById(selectedBook.Id);
            BookForm bookForm = new BookForm(selectedBook);
            bookForm.ShowDialog();
        }

        private void searchTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!searchTextBox.ReadOnly && e.KeyChar == '\r')
            {
                e.Handled = true;
                ProgressOperation searchBookOperation = dataCache.CreateSearchBooksOperation(searchTextBox.Text);
                bookListView.VirtualListDataSource = dataCache.DataAccessor;
                if (searchBookOperation != null)
                {
                    StartProgressOperation(searchBookOperation);
                }
                else
                {
                    ShowBookCountInStatusLabel();
                }
            }
        }

        private void ShowBookCountInStatusLabel()
        {
            string statusLabelPrefix = dataCache.IsInAllBooksMode ? "Всего книг" : "Найдено книг";
            statusLabel.Text = $"{statusLabelPrefix}: {dataCache.DataAccessor.GetObjectCount().ToString("N0", Formatters.BookCountFormat)}";
        }

        private void bookListView_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                case Keys.PageUp:
                case Keys.PageDown:
                case Keys.Home:
                case Keys.End:
                    return;
            }
            if (e.KeyCode == Keys.Escape)
            {
                searchTextBox.Focus();
                searchTextBox.SelectAll();
            }
            e.Handled = true;
            e.SuppressKeyPress = true;
        }
    }
}

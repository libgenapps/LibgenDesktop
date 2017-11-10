namespace LibgenDesktop.Interface
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.fileSubMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.newDatabaseMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openDatabaseMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importFromSqlDumpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.syncMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchTextBox = new System.Windows.Forms.TextBox();
            this.openSqlDumpDialog = new System.Windows.Forms.OpenFileDialog();
            this.bookListView = new BrightIdeasSoftware.VirtualObjectListView();
            this.idColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.titleColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.authorsColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.seriesColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.yearColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.publisherColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.formatColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.fileSizeColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.statusPanel = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.offlineModeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.spacerLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.connectionStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.mainMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bookListView)).BeginInit();
            this.statusPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileSubMenu});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Padding = new System.Windows.Forms.Padding(7, 3, 0, 3);
            this.mainMenu.Size = new System.Drawing.Size(1184, 25);
            this.mainMenu.TabIndex = 0;
            this.mainMenu.Text = "menuStrip1";
            // 
            // fileSubMenu
            // 
            this.fileSubMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newDatabaseMenuItem,
            this.openDatabaseMenuItem,
            this.importFromSqlDumpMenuItem,
            this.syncMenuItem,
            this.offlineModeMenuItem,
            this.exitMenuItem});
            this.fileSubMenu.Name = "fileSubMenu";
            this.fileSubMenu.Size = new System.Drawing.Size(48, 19);
            this.fileSubMenu.Text = "Файл";
            // 
            // newDatabaseMenuItem
            // 
            this.newDatabaseMenuItem.Enabled = false;
            this.newDatabaseMenuItem.Name = "newDatabaseMenuItem";
            this.newDatabaseMenuItem.Size = new System.Drawing.Size(205, 22);
            this.newDatabaseMenuItem.Text = "Новая БД...";
            // 
            // openDatabaseMenuItem
            // 
            this.openDatabaseMenuItem.Enabled = false;
            this.openDatabaseMenuItem.Name = "openDatabaseMenuItem";
            this.openDatabaseMenuItem.Size = new System.Drawing.Size(205, 22);
            this.openDatabaseMenuItem.Text = "Открыть БД...";
            this.openDatabaseMenuItem.Click += new System.EventHandler(this.openDatabaseMenuItem_Click);
            // 
            // importFromSqlDumpMenuItem
            // 
            this.importFromSqlDumpMenuItem.Enabled = false;
            this.importFromSqlDumpMenuItem.Name = "importFromSqlDumpMenuItem";
            this.importFromSqlDumpMenuItem.Size = new System.Drawing.Size(205, 22);
            this.importFromSqlDumpMenuItem.Text = "Импорт из SQL-дампа...";
            this.importFromSqlDumpMenuItem.Click += new System.EventHandler(this.importFromSqlDumpMenuItem_Click);
            // 
            // syncMenuItem
            // 
            this.syncMenuItem.Enabled = false;
            this.syncMenuItem.Name = "syncMenuItem";
            this.syncMenuItem.Size = new System.Drawing.Size(205, 22);
            this.syncMenuItem.Text = "Синхронизация...";
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.Size = new System.Drawing.Size(205, 22);
            this.exitMenuItem.Text = "Выход";
            this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
            // 
            // searchTextBox
            // 
            this.searchTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.searchTextBox.BackColor = System.Drawing.Color.White;
            this.searchTextBox.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.searchTextBox.Location = new System.Drawing.Point(5, 31);
            this.searchTextBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.ReadOnly = true;
            this.searchTextBox.Size = new System.Drawing.Size(1172, 29);
            this.searchTextBox.TabIndex = 2;
            this.searchTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.searchTextBox_KeyPress);
            // 
            // openSqlDumpDialog
            // 
            this.openSqlDumpDialog.Filter = "SQL-дампы (*.sql)|*.sql|Все файлы (*.*)|*.*";
            this.openSqlDumpDialog.Title = "Выбор SQL-дампа";
            // 
            // bookListView
            // 
            this.bookListView.AllColumns.Add(this.idColumn);
            this.bookListView.AllColumns.Add(this.titleColumn);
            this.bookListView.AllColumns.Add(this.authorsColumn);
            this.bookListView.AllColumns.Add(this.seriesColumn);
            this.bookListView.AllColumns.Add(this.yearColumn);
            this.bookListView.AllColumns.Add(this.publisherColumn);
            this.bookListView.AllColumns.Add(this.formatColumn);
            this.bookListView.AllColumns.Add(this.fileSizeColumn);
            this.bookListView.AllowColumnReorder = true;
            this.bookListView.AlternateRowBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.bookListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bookListView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.bookListView.CellEditUseWholeCell = false;
            this.bookListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.idColumn,
            this.titleColumn,
            this.authorsColumn,
            this.seriesColumn,
            this.yearColumn,
            this.publisherColumn,
            this.formatColumn,
            this.fileSizeColumn});
            this.bookListView.Cursor = System.Windows.Forms.Cursors.Default;
            this.bookListView.FullRowSelect = true;
            this.bookListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.bookListView.HeaderUsesThemes = true;
            this.bookListView.IsSearchOnSortColumn = false;
            this.bookListView.Location = new System.Drawing.Point(5, 67);
            this.bookListView.MultiSelect = false;
            this.bookListView.Name = "bookListView";
            this.bookListView.RowHeight = 26;
            this.bookListView.SelectAllOnControlA = false;
            this.bookListView.ShowGroups = false;
            this.bookListView.Size = new System.Drawing.Size(1172, 519);
            this.bookListView.TabIndex = 3;
            this.bookListView.UseAlternatingBackColors = true;
            this.bookListView.UseCompatibleStateImageBehavior = false;
            this.bookListView.UseHotControls = false;
            this.bookListView.UseOverlays = false;
            this.bookListView.UseTranslucentSelection = true;
            this.bookListView.View = System.Windows.Forms.View.Details;
            this.bookListView.VirtualMode = true;
            this.bookListView.DoubleClick += new System.EventHandler(this.bookListView_DoubleClick);
            this.bookListView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.bookListView_KeyDown);
            // 
            // idColumn
            // 
            this.idColumn.AspectName = "Id";
            this.idColumn.Searchable = false;
            this.idColumn.Text = "№";
            // 
            // titleColumn
            // 
            this.titleColumn.AspectName = "Title";
            this.titleColumn.FillsFreeSpace = true;
            this.titleColumn.Searchable = false;
            this.titleColumn.Text = "Наименование";
            // 
            // authorsColumn
            // 
            this.authorsColumn.AspectName = "Authors";
            this.authorsColumn.FillsFreeSpace = true;
            this.authorsColumn.Searchable = false;
            this.authorsColumn.Text = "Авторы";
            // 
            // seriesColumn
            // 
            this.seriesColumn.AspectName = "Series";
            this.seriesColumn.FillsFreeSpace = true;
            this.seriesColumn.Searchable = false;
            this.seriesColumn.Text = "Серия";
            // 
            // yearColumn
            // 
            this.yearColumn.AspectName = "Year";
            this.yearColumn.Searchable = false;
            this.yearColumn.Text = "Год публикации";
            this.yearColumn.Width = 110;
            // 
            // publisherColumn
            // 
            this.publisherColumn.AspectName = "Publisher";
            this.publisherColumn.Searchable = false;
            this.publisherColumn.Text = "Издатель";
            this.publisherColumn.Width = 150;
            // 
            // formatColumn
            // 
            this.formatColumn.AspectName = "Format";
            this.formatColumn.Searchable = false;
            this.formatColumn.Text = "Формат";
            // 
            // fileSizeColumn
            // 
            this.fileSizeColumn.AspectName = "SizeInBytes";
            this.fileSizeColumn.Searchable = false;
            this.fileSizeColumn.Text = "Размер файла";
            this.fileSizeColumn.Width = 100;
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(5, 62);
            this.progressBar.Maximum = 1000;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(1172, 3);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 4;
            this.progressBar.Visible = false;
            // 
            // statusPanel
            // 
            this.statusPanel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.statusPanel.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel,
            this.spacerLabel,
            this.connectionStatusLabel});
            this.statusPanel.Location = new System.Drawing.Point(0, 586);
            this.statusPanel.Name = "statusPanel";
            this.statusPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
            this.statusPanel.Size = new System.Drawing.Size(1184, 25);
            this.statusPanel.TabIndex = 5;
            // 
            // statusLabel
            // 
            this.statusLabel.Margin = new System.Windows.Forms.Padding(1, 3, 0, 5);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(71, 17);
            this.statusLabel.Text = "Status text.";
            // 
            // offlineModeMenuItem
            // 
            this.offlineModeMenuItem.CheckOnClick = true;
            this.offlineModeMenuItem.Name = "offlineModeMenuItem";
            this.offlineModeMenuItem.Size = new System.Drawing.Size(205, 22);
            this.offlineModeMenuItem.Text = "Работать автономно";
            this.offlineModeMenuItem.Click += new System.EventHandler(this.offlineModeMenuItem_Click);
            // 
            // spacerLabel
            // 
            this.spacerLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.spacerLabel.Name = "spacerLabel";
            this.spacerLabel.Size = new System.Drawing.Size(935, 20);
            this.spacerLabel.Spring = true;
            this.spacerLabel.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            // 
            // connectionStatusLabel
            // 
            this.connectionStatusLabel.Name = "connectionStatusLabel";
            this.connectionStatusLabel.Size = new System.Drawing.Size(127, 20);
            this.connectionStatusLabel.Text = "Автономный режим";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1184, 611);
            this.Controls.Add(this.statusPanel);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.bookListView);
            this.Controls.Add(this.searchTextBox);
            this.Controls.Add(this.mainMenu);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.MainMenuStrip = this.mainMenu;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Libgen Desktop";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bookListView)).EndInit();
            this.statusPanel.ResumeLayout(false);
            this.statusPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileSubMenu;
        private System.Windows.Forms.TextBox searchTextBox;
        private System.Windows.Forms.ToolStripMenuItem newDatabaseMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openDatabaseMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importFromSqlDumpMenuItem;
        private System.Windows.Forms.ToolStripMenuItem syncMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private System.Windows.Forms.OpenFileDialog openSqlDumpDialog;
        private BrightIdeasSoftware.OLVColumn idColumn;
        private BrightIdeasSoftware.OLVColumn titleColumn;
        private BrightIdeasSoftware.OLVColumn authorsColumn;
        private BrightIdeasSoftware.OLVColumn seriesColumn;
        private BrightIdeasSoftware.OLVColumn yearColumn;
        private BrightIdeasSoftware.OLVColumn publisherColumn;
        private BrightIdeasSoftware.OLVColumn formatColumn;
        private BrightIdeasSoftware.OLVColumn fileSizeColumn;
        private BrightIdeasSoftware.VirtualObjectListView bookListView;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.StatusStrip statusPanel;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ToolStripMenuItem offlineModeMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel spacerLabel;
        private System.Windows.Forms.ToolStripStatusLabel connectionStatusLabel;
    }
}


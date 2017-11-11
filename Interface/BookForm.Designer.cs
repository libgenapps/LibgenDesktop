namespace LibgenDesktop.Interface
{
    partial class BookForm
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.valueLabelContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyValueLabelTextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bookDetailsDataGridView = new System.Windows.Forms.DataGridView();
            this.downloadButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.coverPanel = new System.Windows.Forms.Panel();
            this.coverLoadingLabel = new System.Windows.Forms.Label();
            this.bookCoverPictureBox = new System.Windows.Forms.PictureBox();
            this.titleColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.valueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.valueLabelContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bookDetailsDataGridView)).BeginInit();
            this.coverPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bookCoverPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // valueLabelContextMenu
            // 
            this.valueLabelContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyValueLabelTextMenuItem});
            this.valueLabelContextMenu.Name = "valueLabelContextMenu";
            this.valueLabelContextMenu.Size = new System.Drawing.Size(147, 26);
            // 
            // copyValueLabelTextMenuItem
            // 
            this.copyValueLabelTextMenuItem.Name = "copyValueLabelTextMenuItem";
            this.copyValueLabelTextMenuItem.Size = new System.Drawing.Size(146, 22);
            this.copyValueLabelTextMenuItem.Text = "Скопировать";
            this.copyValueLabelTextMenuItem.Click += new System.EventHandler(this.copyValueLabelTextMenuItem_Click);
            // 
            // bookDetailsDataGridView
            // 
            this.bookDetailsDataGridView.AllowUserToAddRows = false;
            this.bookDetailsDataGridView.AllowUserToDeleteRows = false;
            this.bookDetailsDataGridView.AllowUserToResizeRows = false;
            this.bookDetailsDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bookDetailsDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.bookDetailsDataGridView.BackgroundColor = System.Drawing.Color.White;
            this.bookDetailsDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.bookDetailsDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.bookDetailsDataGridView.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.bookDetailsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.bookDetailsDataGridView.ColumnHeadersVisible = false;
            this.bookDetailsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.titleColumn,
            this.valueColumn});
            this.bookDetailsDataGridView.Location = new System.Drawing.Point(311, 11);
            this.bookDetailsDataGridView.Name = "bookDetailsDataGridView";
            this.bookDetailsDataGridView.ReadOnly = true;
            this.bookDetailsDataGridView.RowHeadersVisible = false;
            this.bookDetailsDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.bookDetailsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.bookDetailsDataGridView.Size = new System.Drawing.Size(862, 518);
            this.bookDetailsDataGridView.TabIndex = 100;
            this.bookDetailsDataGridView.CellContextMenuStripNeeded += new System.Windows.Forms.DataGridViewCellContextMenuStripNeededEventHandler(this.bookDetailsDataGridView_CellContextMenuStripNeeded);
            this.bookDetailsDataGridView.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
            // 
            // downloadButton
            // 
            this.downloadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.downloadButton.Location = new System.Drawing.Point(867, 535);
            this.downloadButton.Name = "downloadButton";
            this.downloadButton.Size = new System.Drawing.Size(150, 33);
            this.downloadButton.TabIndex = 93;
            this.downloadButton.Text = "Скачать с libgen.io";
            this.downloadButton.UseVisualStyleBackColor = true;
            this.downloadButton.Click += new System.EventHandler(this.downloadButton_Click);
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Location = new System.Drawing.Point(1023, 535);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(150, 33);
            this.closeButton.TabIndex = 94;
            this.closeButton.Text = "Закрыть";
            this.closeButton.UseVisualStyleBackColor = true;
            // 
            // coverPanel
            // 
            this.coverPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.coverPanel.Controls.Add(this.coverLoadingLabel);
            this.coverPanel.Controls.Add(this.bookCoverPictureBox);
            this.coverPanel.Location = new System.Drawing.Point(11, 11);
            this.coverPanel.Name = "coverPanel";
            this.coverPanel.Size = new System.Drawing.Size(294, 521);
            this.coverPanel.TabIndex = 101;
            // 
            // coverLoadingLabel
            // 
            this.coverLoadingLabel.BackColor = System.Drawing.Color.Transparent;
            this.coverLoadingLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.coverLoadingLabel.Location = new System.Drawing.Point(0, 0);
            this.coverLoadingLabel.Name = "coverLoadingLabel";
            this.coverLoadingLabel.Size = new System.Drawing.Size(294, 521);
            this.coverLoadingLabel.TabIndex = 95;
            this.coverLoadingLabel.Text = "Загружается обложка...";
            this.coverLoadingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bookCoverPictureBox
            // 
            this.bookCoverPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bookCoverPictureBox.Location = new System.Drawing.Point(0, 0);
            this.bookCoverPictureBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.bookCoverPictureBox.Name = "bookCoverPictureBox";
            this.bookCoverPictureBox.Size = new System.Drawing.Size(294, 521);
            this.bookCoverPictureBox.TabIndex = 0;
            this.bookCoverPictureBox.TabStop = false;
            // 
            // titleColumn
            // 
            this.titleColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.titleColumn.DataPropertyName = "Title";
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(2);
            this.titleColumn.DefaultCellStyle = dataGridViewCellStyle3;
            this.titleColumn.HeaderText = "titleColumn";
            this.titleColumn.Name = "titleColumn";
            this.titleColumn.ReadOnly = true;
            this.titleColumn.Width = 5;
            // 
            // valueColumn
            // 
            this.valueColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.valueColumn.DataPropertyName = "Value";
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.Padding = new System.Windows.Forms.Padding(0, 2, 2, 2);
            this.valueColumn.DefaultCellStyle = dataGridViewCellStyle4;
            this.valueColumn.HeaderText = "valueColumn";
            this.valueColumn.Name = "valueColumn";
            this.valueColumn.ReadOnly = true;
            // 
            // BookForm
            // 
            this.AcceptButton = this.downloadButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(1184, 579);
            this.Controls.Add(this.coverPanel);
            this.Controls.Add(this.downloadButton);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.bookDetailsDataGridView);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "BookForm";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "BookForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.BookForm_FormClosed);
            this.Load += new System.EventHandler(this.BookForm_Load);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.BookForm_MouseMove);
            this.valueLabelContextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bookDetailsDataGridView)).EndInit();
            this.coverPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bookCoverPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button downloadButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.ContextMenuStrip valueLabelContextMenu;
        private System.Windows.Forms.ToolStripMenuItem copyValueLabelTextMenuItem;
        private System.Windows.Forms.DataGridView bookDetailsDataGridView;
        private System.Windows.Forms.Panel coverPanel;
        private System.Windows.Forms.Label coverLoadingLabel;
        private System.Windows.Forms.PictureBox bookCoverPictureBox;
        private System.Windows.Forms.DataGridViewTextBoxColumn titleColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn valueColumn;
    }
}
namespace LibgenDesktop.Interface
{
    partial class ProgressForm
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
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.progressDescription = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.estimatedTimeLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 32);
            this.progressBar.Maximum = 1000;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(593, 23);
            this.progressBar.TabIndex = 0;
            // 
            // progressDescription
            // 
            this.progressDescription.AutoEllipsis = true;
            this.progressDescription.Location = new System.Drawing.Point(12, 9);
            this.progressDescription.Name = "progressDescription";
            this.progressDescription.Size = new System.Drawing.Size(593, 20);
            this.progressDescription.TabIndex = 1;
            this.progressDescription.Text = "Progress description";
            this.progressDescription.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(498, 61);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(107, 31);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Прервать";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // estimatedTimeLabel
            // 
            this.estimatedTimeLabel.AutoSize = true;
            this.estimatedTimeLabel.Location = new System.Drawing.Point(9, 68);
            this.estimatedTimeLabel.Name = "estimatedTimeLabel";
            this.estimatedTimeLabel.Size = new System.Drawing.Size(94, 17);
            this.estimatedTimeLabel.TabIndex = 3;
            this.estimatedTimeLabel.Text = "Estimated time";
            // 
            // ProgressForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(610, 98);
            this.ControlBox = false;
            this.Controls.Add(this.estimatedTimeLabel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.progressDescription);
            this.Controls.Add(this.progressBar);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(626, 137);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(626, 137);
            this.Name = "ProgressForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ProgressForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProgressForm_FormClosing);
            this.Shown += new System.EventHandler(this.ProgressForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label progressDescription;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label estimatedTimeLabel;
    }
}
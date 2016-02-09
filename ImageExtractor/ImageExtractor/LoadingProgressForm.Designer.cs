namespace ImageExtractor
{
    partial class LoadingProgressForm
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
            this.lblStatus = new System.Windows.Forms.Label();
            this.pbStatus = new System.Windows.Forms.PictureBox();
            this.bwLoading = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.pbStatus)).BeginInit();
            this.SuspendLayout();
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(87, 42);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(99, 15);
            this.lblStatus.TabIndex = 6;
            this.lblStatus.Text = "Loading image...";
            // 
            // pbStatus
            // 
            this.pbStatus.Image = global::ImageExtractor.Properties.Resources.InformationImage;
            this.pbStatus.Location = new System.Drawing.Point(23, 27);
            this.pbStatus.Name = "pbStatus";
            this.pbStatus.Size = new System.Drawing.Size(48, 48);
            this.pbStatus.TabIndex = 7;
            this.pbStatus.TabStop = false;
            // 
            // bwLoading
            // 
            this.bwLoading.WorkerReportsProgress = true;
            this.bwLoading.WorkerSupportsCancellation = true;
            this.bwLoading.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwLoading_OnDoWork);
            this.bwLoading.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwLoading_OnRunWorkerCompleted);
            // 
            // LoadingProgressForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(445, 120);
            this.Controls.Add(this.pbStatus);
            this.Controls.Add(this.lblStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "LoadingProgressForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Loading Progress";
            ((System.ComponentModel.ISupportInitialize)(this.pbStatus)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.PictureBox pbStatus;
        private System.ComponentModel.BackgroundWorker bwLoading;
    }
}
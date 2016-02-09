using System;
namespace ImageExtractor
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
            this.components = new System.ComponentModel.Container();
            this.lblSearchTerm = new System.Windows.Forms.Label();
            this.txtSearchTerm = new System.Windows.Forms.TextBox();
            this.lblPage = new System.Windows.Forms.Label();
            this.lblNoSearchTerm = new System.Windows.Forms.Label();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.lblNoImages = new System.Windows.Forms.Label();
            this.clickTimer = new System.Windows.Forms.Timer(this.components);
            this.pbExit = new System.Windows.Forms.PictureBox();
            this.pbSearch = new System.Windows.Forms.PictureBox();
            this.pbPrevious = new System.Windows.Forms.PictureBox();
            this.pbNext = new System.Windows.Forms.PictureBox();
            this.pbSave = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbExit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbPrevious)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbNext)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSave)).BeginInit();
            this.SuspendLayout();
            // 
            // lblSearchTerm
            // 
            this.lblSearchTerm.AutoSize = true;
            this.lblSearchTerm.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSearchTerm.Location = new System.Drawing.Point(99, 37);
            this.lblSearchTerm.Name = "lblSearchTerm";
            this.lblSearchTerm.Size = new System.Drawing.Size(129, 18);
            this.lblSearchTerm.TabIndex = 0;
            this.lblSearchTerm.Text = "Search Concept";
            // 
            // txtSearchTerm
            // 
            this.txtSearchTerm.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearchTerm.Location = new System.Drawing.Point(233, 34);
            this.txtSearchTerm.Name = "txtSearchTerm";
            this.txtSearchTerm.Size = new System.Drawing.Size(661, 24);
            this.txtSearchTerm.TabIndex = 1;
            // 
            // lblPage
            // 
            this.lblPage.AutoSize = true;
            this.lblPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPage.Location = new System.Drawing.Point(534, 543);
            this.lblPage.Name = "lblPage";
            this.lblPage.Size = new System.Drawing.Size(51, 18);
            this.lblPage.TabIndex = 13;
            this.lblPage.Text = "Page ";
            // 
            // lblNoSearchTerm
            // 
            this.lblNoSearchTerm.AutoSize = true;
            this.lblNoSearchTerm.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNoSearchTerm.ForeColor = System.Drawing.Color.Red;
            this.lblNoSearchTerm.Location = new System.Drawing.Point(230, 61);
            this.lblNoSearchTerm.Name = "lblNoSearchTerm";
            this.lblNoSearchTerm.Size = new System.Drawing.Size(184, 15);
            this.lblNoSearchTerm.TabIndex = 14;
            this.lblNoSearchTerm.Text = "Please enter a search term!";
            this.lblNoSearchTerm.Visible = false;
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "csv";
            this.openFileDialog.Filter = "CSV files (*.csv)|*.csv";
            this.openFileDialog.InitialDirectory = "C:\\Users\\Matthew\\Desktop";
            // 
            // lblNoImages
            // 
            this.lblNoImages.AutoSize = true;
            this.lblNoImages.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNoImages.ForeColor = System.Drawing.Color.Red;
            this.lblNoImages.Location = new System.Drawing.Point(96, 93);
            this.lblNoImages.Name = "lblNoImages";
            this.lblNoImages.Size = new System.Drawing.Size(503, 24);
            this.lblNoImages.TabIndex = 18;
            this.lblNoImages.Text = "No Images matching your search criteria were found!";
            this.lblNoImages.Visible = false;
            // 
            // clickTimer
            // 
            this.clickTimer.Tick += new System.EventHandler(this.clickTimer_Tick);
            // 
            // pbExit
            // 
            this.pbExit.Image = global::ImageExtractor.Properties.Resources.ExitImage;
            this.pbExit.Location = new System.Drawing.Point(971, 21);
            this.pbExit.Name = "pbExit";
            this.pbExit.Size = new System.Drawing.Size(48, 48);
            this.pbExit.TabIndex = 26;
            this.pbExit.TabStop = false;
            this.pbExit.Click += new System.EventHandler(this.pbExit_Click);
            this.pbExit.MouseHover += new System.EventHandler(this.pbExit_MouseHover);
            // 
            // pbSearch
            // 
            this.pbSearch.Image = global::ImageExtractor.Properties.Resources.SearchImage;
            this.pbSearch.Location = new System.Drawing.Point(911, 21);
            this.pbSearch.Name = "pbSearch";
            this.pbSearch.Size = new System.Drawing.Size(48, 48);
            this.pbSearch.TabIndex = 25;
            this.pbSearch.TabStop = false;
            this.pbSearch.Click += new System.EventHandler(this.pbSearch_Click);
            this.pbSearch.MouseHover += new System.EventHandler(this.pbSearch_MouseHover);
            // 
            // pbPrevious
            // 
            this.pbPrevious.Image = global::ImageExtractor.Properties.Resources.PreviousImage;
            this.pbPrevious.Location = new System.Drawing.Point(911, 529);
            this.pbPrevious.Name = "pbPrevious";
            this.pbPrevious.Size = new System.Drawing.Size(48, 48);
            this.pbPrevious.TabIndex = 24;
            this.pbPrevious.TabStop = false;
            this.pbPrevious.Click += new System.EventHandler(this.pbPrevious_Click);
            this.pbPrevious.MouseHover += new System.EventHandler(this.pbPrevious_MouseHover);
            // 
            // pbNext
            // 
            this.pbNext.Image = global::ImageExtractor.Properties.Resources.NextImage;
            this.pbNext.Location = new System.Drawing.Point(971, 529);
            this.pbNext.Name = "pbNext";
            this.pbNext.Size = new System.Drawing.Size(48, 48);
            this.pbNext.TabIndex = 23;
            this.pbNext.TabStop = false;
            this.pbNext.Click += new System.EventHandler(this.pbNext_Click);
            this.pbNext.MouseHover += new System.EventHandler(this.pbNext_MouseHover);
            // 
            // pbSave
            // 
            this.pbSave.Image = global::ImageExtractor.Properties.Resources.SavingImage;
            this.pbSave.Location = new System.Drawing.Point(1032, 529);
            this.pbSave.Name = "pbSave";
            this.pbSave.Size = new System.Drawing.Size(48, 48);
            this.pbSave.TabIndex = 22;
            this.pbSave.TabStop = false;
            this.pbSave.Click += new System.EventHandler(this.pbSave_Click);
            this.pbSave.MouseHover += new System.EventHandler(this.pbSave_MouseHover);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1134, 603);
            this.Controls.Add(this.pbExit);
            this.Controls.Add(this.pbSearch);
            this.Controls.Add(this.pbPrevious);
            this.Controls.Add(this.pbNext);
            this.Controls.Add(this.pbSave);
            this.Controls.Add(this.lblNoImages);
            this.Controls.Add(this.lblNoSearchTerm);
            this.Controls.Add(this.lblPage);
            this.Controls.Add(this.txtSearchTerm);
            this.Controls.Add(this.lblSearchTerm);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Image Extractor";
            ((System.ComponentModel.ISupportInitialize)(this.pbExit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbPrevious)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbNext)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSave)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSearchTerm;
        private System.Windows.Forms.TextBox txtSearchTerm;
        private System.Windows.Forms.Label lblPage;
        private System.Windows.Forms.Label lblNoSearchTerm;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Label lblNoImages;
        private System.Windows.Forms.PictureBox pbSave;
        private System.Windows.Forms.PictureBox pbNext;
        private System.Windows.Forms.PictureBox pbPrevious;
        private System.Windows.Forms.PictureBox pbSearch;
        private System.Windows.Forms.PictureBox pbExit;
        private System.Windows.Forms.Timer clickTimer;
    }
}


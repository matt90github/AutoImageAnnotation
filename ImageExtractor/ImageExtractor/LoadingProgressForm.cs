//Defining the class libraries
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace ImageExtractor
{
    //A form showing the loading progress of an image when the user double clicks an image from the main form
    public partial class LoadingProgressForm : Form
    {
        //The image URL of the full sized image
        private string imageURL;

        //An instance of the image detail form
        private ImageDetailForm imageDetailForm;

        //Default Class Contructor
        public LoadingProgressForm()
        {
            InitializeComponent();
        }

        //Overloaded Constructor taking the full image URL as input
        public LoadingProgressForm(string fullImageURL)
        {
            InitializeComponent();

            //Initialise the URL
            imageURL = fullImageURL;

            //Starts execution of the background operation associated with the background worker object
            bwLoading.RunWorkerAsync();
        }

        //OnDoWork Event of the background worker object
        private void bwLoading_OnDoWork(object sender, DoWorkEventArgs e)
        {
            //Initialising the image detail form
            imageDetailForm = new ImageDetailForm(imageURL);
        }

        //OnRunWorkerCompleted Event of the background worker object
        private void bwLoading_OnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                //Show the image once loading has taken place
                imageDetailForm.ShowDialog();

                //Close the loading progress form and release the associated resources
                this.Close();
                this.Dispose();
            }
            catch (ObjectDisposedException)
            {
                //The user is informed accordingly when the image fails to load
                lblStatus.Text = "Failed to load image";

                //Change the PictureBox icon to an error image
                pbStatus.Image = Properties.Resources.ErrorImage;
            }
        }
    }
}

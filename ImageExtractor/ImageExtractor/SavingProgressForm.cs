//Defining the class libraries
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Net;
using System.Windows.Forms;

namespace ImageExtractor
{
    //A form showing the saving progress of the selected page images
    public partial class SavingProgressForm : Form
    {
        //A list of full sized image URLs
        List<string> fullImagesList;

        //A Boolean array to specify whether an image is selected or not
        Boolean[] isSelected;

        //The search text entered by the user
        string searchTerms;

        //The location where the images will be saved
        string chosenPath;

        //The page number, number of successful saves, current save counter and number of selected images
        int pageNo, successfulCounter, currentCounter, numberOfSelectedImages;

        //Indicates whether a saving issue was encountered
        bool savingIssue;

        //Default Class Contructor
        public SavingProgressForm()
        {
            InitializeComponent();
        }

        //Overloaded Constructor taking the full image URL list, the Boolean array, search text, page number and selected path as inputs
        public SavingProgressForm(List<string> fullImages, Boolean[] imageSelected, string searchText, int pageNumber, string selectedPath)
        {
            InitializeComponent();

            //Initialise the variables
            fullImagesList = fullImages;
            isSelected = imageSelected;
            searchTerms = searchText;
            pageNo = pageNumber;
            chosenPath = selectedPath;

            //Change the PictureBox icon to an information image
            pbStatus.Image = Properties.Resources.InformationImage;

            btnOK.Enabled = false;
            btnCancel.Enabled = true;

            //Starts execution of the background operation associated with the background worker object
            bwSaving.RunWorkerAsync();
        }

        //The Click Event for the Ok button
        private void btnOK_Click(object sender, EventArgs e)
        {
            //Close the window and release the associated resources
            this.Close();
            this.Dispose();
        }

        //The Click Event for the Cancel button
        private void btnCancel_Click(object sender, EventArgs e)
        {
            //Requests cancellation of the pending background operation
            bwSaving.CancelAsync();
        }

        //OnDoWork Event of the background worker object
        private void bwSaving_OnDoWork(object sender, DoWorkEventArgs e)
        {
            savingIssue = false;
            successfulCounter = 0;
            currentCounter = 0;
            numberOfSelectedImages = 0;

            PictureBox[] fullImgArray;

            if (fullImagesList != null)
            {
                for (int i = 0; i < fullImagesList.Count; i++)
                {
                    //Check if the current image is selected
                    if (isSelected[i])
                        numberOfSelectedImages++;
                }
                
                //Initialise the PictureBox array
                fullImgArray = new PictureBox[fullImagesList.Count];

                for (int i = 0; i < fullImagesList.Count; i++)
                {
                    //Check if the user has cancelled the saving operation
                    if (bwSaving.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }

                    fullImgArray[i] = new PictureBox();

                    try
                    {
                        if(isSelected[i])
                        {
                            currentCounter++;

                            //Load the full sized image
                            fullImgArray[i].Load(fullImagesList[i]);

                            //Remove spaces from the user's search query
                            string searchTermsNoSpaces = searchTerms.Replace(" ", "");

                            //Save the current image in JPG format
                            fullImgArray[i].Image.Save(chosenPath + "/" + searchTermsNoSpaces + ((i + 1) + (pageNo * 10)) + ".jpg", ImageFormat.Jpeg);

                            //Increase the successful counter
                            successfulCounter++;

                            //Change the PictureBox icon to an animated spinner image
                            pbStatus.Image = Properties.Resources.AnimatedImage;

                            //Display the current saving progress
                            bwSaving.ReportProgress(-1, string.Format("Saving image " + currentCounter + "/" + numberOfSelectedImages + "..."));
                        }
                    }
                    catch (ArgumentException)
                    {
                        savingIssue = true;
                    }
                    catch (WebException)
                    {
                        savingIssue = true;
                    }
                }
            }
            else
            {
                //In the case that no images were selected to be saved
                savingIssue = true;
                throw new Exception("No images to save!");
            }
        }

        //OnProgressChanged Event of the background worker object
        private void bwSaving_OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //Changes the label text in the case that the 
            if (e.UserState is String)
            {
                lblStatus.Text = (String)e.UserState;
            }
        }

        //OnRunWorkerCompleted Event of the background worker object
        private void bwSaving_OnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pbStatus.Image = null;

            //If the saving operation was cancelled by the user
            if (e.Cancelled)
            {
                lblStatus.Text = "Operation cancelled by the user!";

                //Change the PictureBox icon to a warning image
                pbStatus.Image = Properties.Resources.WarningImage;
            }
            else
            {
                //If an error was encountered while saving an image
                if (e.Error != null)
                {
                    lblStatus.Text = "Operation failed: " + e.Error.Message;

                    //Change the PictureBox icon to an error image
                    pbStatus.Image = Properties.Resources.ErrorImage;
                }
                else
                {
                    //If not all the images were saved successfully
                    if(savingIssue)
                        lblStatus.Text = successfulCounter + " out of " + numberOfSelectedImages + " images were saved successfully!";
                    else
                        lblStatus.Text = "The images were saved successfully!";

                    //Change the PictureBox icon to an information image
                    pbStatus.Image = Properties.Resources.InformationImage;
                }
            }

            btnOK.Enabled = true;
            btnCancel.Enabled = false;
        }

    }
}

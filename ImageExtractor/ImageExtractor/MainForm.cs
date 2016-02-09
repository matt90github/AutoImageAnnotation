//Defining the class libraries
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Google.Apis.Customsearch.v1;
using Google.Apis.Customsearch.v1.Data;

namespace ImageExtractor
{
    //The main form of the image extraction application
    public partial class MainForm : Form
    {
        //Variables storing the current page number and the number of selected images respectively
        public int pageNumber, selectedImages;

        //A PictureBox array storing the thumbnail version of each image
        private PictureBox[] thumbImgArray;

        //Variable storing the selected PictureBox (single or double clicks)
        private PictureBox pbSelected;

        //A Boolean array that indicates whether an image is selected or not
        private Boolean[] imageSelected;

        //Keeps track of whether or not a single click has been made
        private bool isSingleClick;

        //Stores the search text entered by the user
        private string searchText;

        //The directory selected by the user to save the images
        private string selectedPath;

        //Two lists containing image URLs of the thumbnail and full versions of the image respectively
        private List<string> thumbnailImages, fullImages;

        //The API Key associated with the project created on the Google Developers Console
        private const string apiKey = "AIzaSyCWlE6mBU1J6pzTY2o4wcYolLbmbzdAv1A";

        //The Search Engine ID associated with the search engine created on the Google Custom Search Engine portal
        private const string searchEngineId = "004199751463844698286:7kqi5_04ck0";

        //Default Class Contructor, initialising variables and hiding the main PictureBox buttons 
        public MainForm()
        {
            InitializeComponent();
            pageNumber = 0;
            selectedImages = 0;
            selectedPath = "";
            isSingleClick = false;
            pbPrevious.Visible = false;
            pbNext.Visible = false;
            pbSave.Visible = false;
            lblPage.Visible = false;
        }

        //Handling various key strokes while using the application
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //Press Enter to perform a search
            if (keyData == Keys.Enter && pbSearch.Enabled)
            {
                pbSearch_Click(pbSearch, null);
                return true;
            }

            //Press the left arrow to go to the previous page
            if (keyData == Keys.Left && pbPrevious.Visible && pbSearch.Enabled && pbPrevious.Enabled)
            {
                pbPrevious_Click(pbPrevious, null);
                return true;
            }

            //Press the right arrow to go to the next page
            if (keyData == Keys.Right && pbNext.Visible && pbSearch.Enabled && pbNext.Enabled)
            {
                pbNext_Click(pbNext, null);
                return true;
            }

            //Press Ctrl+S to save the selected images on the current page
            if (keyData == (Keys.Control | Keys.S) && pbSave.Enabled && pbSave.Visible)
            {
                pbSave_Click(pbSave, null);
                return true;
            }

            //Press Esc to quit the application
            if (keyData == Keys.Escape)
            {
                pbExit_Click(pbExit, null);
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void performSearch()
        {
            //Clears the content of each existing PictureBox
            this.Clear();
            
            //Temporarily disable the search button
            pbSearch.Enabled = false;

            //Storing the search text entered by the user
            searchText = txtSearchTerm.Text;

            //Displaying the current page number
            lblPage.Text = "Page " + Convert.ToInt32(pageNumber + 1);

            //Initialising both lists
            thumbnailImages = new List<string>();
            fullImages = new List<string>();

            //Defining a new Google Custom Search service using the specific API Key
            CustomsearchService customSearchService = new CustomsearchService(new Google.Apis.Services.BaseClientService.Initializer() { ApiKey = apiKey });

            //Providing the search text to the custom search service
            CseResource.ListRequest listRequest = customSearchService.Cse.List(txtSearchTerm.Text);

            //Associating the list request to the Google Custom Search Engine ID
            listRequest.Cx = searchEngineId;

            //Specifying that the search will be an image search
            listRequest.SearchType = CseResource.ListRequest.SearchTypeEnum.Image;

            //Determining the start position of the search
            listRequest.Start = (pageNumber * 10) + 1;

            //Each search will return a maximum of 10 images (Google Custom Search API limit)
            listRequest.Num = 10;

            try
            {
                //Attempt to perform the search
                Search search = listRequest.Execute();

                //If any search results are returned
                if (search.Items != null)
                {
                    lblNoImages.Visible = false;

                    //Adding the image URLs for both thumbnail and full versions of the image
                    foreach (var item in search.Items)
                    {
                        thumbnailImages.Add(item.Image.ThumbnailLink);
                        fullImages.Add(item.Link);
                    }

                    //Calculating the number of selected images (all images are already selected by default)
                    selectedImages = thumbnailImages.Count;

                    if (thumbnailImages.Count > 0)
                    {
                        //The starting x and y coordinates to draw the first PictureBox
                        int xPos = 50;
                        int yPos = 90;

                        pbSave.Enabled = true;

                        //Initialising the thumbnail PictureBox array
                        thumbImgArray = new PictureBox[thumbnailImages.Count];

                        //Initialising the Boolean array
                        imageSelected = new Boolean[thumbnailImages.Count];

                        for (int i = 0; i < thumbnailImages.Count; i++)
                        {
                            imageSelected[i] = true;
                            thumbImgArray[i] = new PictureBox();

                            //Load the PictureBox with the image having the specified thumbnail image URL
                            thumbImgArray[i].Load(thumbnailImages[i]);

                            //Set the starting position to draw the current PictureBox
                            if (xPos > 1000)
                            {
                                xPos = 50;
                                yPos = yPos + 208;
                            }

                            //Set the properties of the current PictureBox, including width, height, background colour and mouse click events
                            thumbImgArray[i].Left = xPos;
                            thumbImgArray[i].Top = yPos;
                            thumbImgArray[i].Width = 200;
                            thumbImgArray[i].Height = 200;
                            thumbImgArray[i].Visible = true;
                            thumbImgArray[i].BorderStyle = BorderStyle.None;
                            thumbImgArray[i].BackColor = System.Drawing.Color.Green;
                            thumbImgArray[i].Padding = new System.Windows.Forms.Padding(3);
                            thumbImgArray[i].SizeMode = PictureBoxSizeMode.StretchImage;
                            thumbImgArray[i].Tag = i;
                            thumbImgArray[i].MouseDown += new MouseEventHandler(thumbnail_MouseDown);
                            thumbImgArray[i].DoubleClick += new EventHandler(thumbnail_DoubleClick);

                            //Add the PictureBox to the Form controls
                            this.Controls.Add(thumbImgArray[i]);

                            //Sets the x-coordinate for the next PictureBox 
                            xPos = xPos + 208;

                            //Processes all Windows messages currently in the message queue.
                            Application.DoEvents();
                        }

                        pbPrevious.Visible = false;
                        lblPage.Visible = true;
                        pbSave.Visible = true;
                        pbNext.Visible = true;

                        //Re-enable the search button
                        pbSearch.Enabled = true;

                        //Checks the number of thumbnail images to determine whether the 'Next' PictureBox button should show or not
                        if (10 % thumbnailImages.Count != 0)
                            pbNext.Visible = false;
                        else
                            pbNext.Visible = true;
                    }
                }

                //No results found - the user is informed accordingly and the main PictureBox buttons are hidden
                else
                {
                    lblNoImages.Visible = true;
                    pbPrevious.Visible = false;
                    pbNext.Visible = false;
                    lblPage.Visible = false;
                    pbSave.Visible = false;
                }
            }

            catch (Google.GoogleApiException)
            {
                //The user is informed accordingly if a Google API exception is thrown
                lblNoImages.Text = "Google Custom Search API Issue";
                lblNoImages.Visible = true;
            }            
        }

        //Class method which is invoked for every search performed in order to remove and release the resources associated with the PictureBoxes
        private void Clear()
        {
            if (thumbImgArray != null)
            {
                foreach (var pictureBox in thumbImgArray)
                {
                    pictureBox.Image = null;
                    pictureBox.Invalidate();
                    pictureBox.Dispose();
                }
            }
        }

        //Takes place whenever a PictureBox is clicked by the user
        private void thumbnail_MouseDown(object sender, MouseEventArgs e)
        {
            //Retrieve the selected PictureBox
            pbSelected = sender as PictureBox;

            if (e.Button == MouseButtons.Left)
            {
                //The click is deemed to be a single click if no more than one left mouse button click is performed within the predefined timer interval
                if (e.Clicks < 2)
                {
                    //Set the timer interval
                    clickTimer.Interval = SystemInformation.DoubleClickTime;

                    //Start the timer
                    clickTimer.Start();

                    isSingleClick = true;
                }
                else if (e.Clicks == 2)
                {
                    //Stop the timer
                    clickTimer.Stop();

                    isSingleClick = false;
                }
            }
        }

        //The Timer Tick Event
        private void clickTimer_Tick(object sender, EventArgs e)
        {
            //If a single click is detected
            if (isSingleClick)
            {
                //Stop the timer
                clickTimer.Stop();

                isSingleClick = false;

                //Call the single click event
                thumbnail_Click(sender, e);
            }
        }

        //Takes place whenever a PictureBox is single clicked by the user
        protected void thumbnail_Click(object sender, EventArgs e)
        {
            //Retrieve the image index from the Tag attribute
            int index = Convert.ToInt32(pbSelected.Tag.ToString());

            //Change the PictoreBox from selected to unselected
            if (pbSelected.BackColor == Color.Green)
            {
                pbSelected.BackColor = Color.Red;
                selectedImages--;
                imageSelected[index] = false;
            }

            //Change the PictoreBox from unselected to selected
            else if (pbSelected.BackColor == Color.Red)
            {
                pbSelected.BackColor = Color.Green;
                selectedImages++;
                imageSelected[index] = true;
            }

            //Determine whether the 'Save' PictureBox button should be enabled or not
            if (selectedImages == 0)
                pbSave.Enabled = false;
            else
                pbSave.Enabled = true;
        }

        //Takes place whenever a PictureBox is double clicked by the user
        protected void thumbnail_DoubleClick(object sender, EventArgs e)
        {
            //Retrieve the selected PictureBox
            PictureBox pbSelected = sender as PictureBox;

            //Retrieve the image index from the Tag attribute
            int index = Convert.ToInt32(pbSelected.Tag.ToString());
            
            //Get the image URL of the full sized image
            string fullImageURL = fullImages[index].ToString();

            //Create a new instance of the loading progress form, passing the full image URL as input
            LoadingProgressForm progressForm = new LoadingProgressForm(fullImageURL);

            //Show the loading progress form
            progressForm.ShowDialog();
        }

        //The Click Event for the 'Save' PictureBox button
        private void pbSave_Click(object sender, EventArgs e)
        {
            //If the user is saving for the first time
            if (String.IsNullOrEmpty(selectedPath))
            {
                // Displays a FolderBrowserDialog so that the user could choose where to save the images
                FolderBrowserDialog fbDialog = new FolderBrowserDialog();

                //The message shown to the user
                fbDialog.Description = "Select the directory where you want to save the images";

                //Set the default folder to 'My Documents'
                fbDialog.RootFolder = Environment.SpecialFolder.MyDocuments;

                // Show the FolderBrowserDialog
                DialogResult result = fbDialog.ShowDialog();

                //Get the selsected directory
                if (result == DialogResult.OK)
                {
                    selectedPath = fbDialog.SelectedPath;
                }
            }
           
            // If the file name is not an empty string open it for saving
            if (!String.IsNullOrEmpty(selectedPath))
            {
                //Create a new instance of the saving progress form, passing the full image URLs, selected images, search text, page number and selected path as input
                SavingProgressForm progressForm = new SavingProgressForm(fullImages, imageSelected, searchText, pageNumber, selectedPath);

                //Show the saving progress form
                progressForm.ShowDialog();
            }
        }

        //The Click Event for the 'Next' PictureBox button
        private void pbNext_Click(object sender, EventArgs e)
        {
            if(pbSearch.Enabled)
            { 
                pageNumber++;

                if ((pageNumber + 1) <= 10)
                {
                    //Invoke the search method
                    performSearch();
                }

                //Disable the next button if the Google Custom Search API limit of 100 images has been reached
                if ((pageNumber+1) == 10)
                    pbNext.Enabled = false;

                if (pageNumber != 0)
                    pbPrevious.Visible = true;
                else
                    pbPrevious.Visible = false;
            }
        }

        //The Click Event for the 'Previous' PictureBox button
        private void pbPrevious_Click(object sender, EventArgs e)
        {
            if (pbSearch.Enabled)
            {
                pageNumber--;
                pbNext.Enabled = true;

                //Invoke the search method
                performSearch();

                if (pageNumber != 0)
                    pbPrevious.Visible = true;
                else
                    pbPrevious.Visible = false;
            }
        }

        //The Click Event for the 'Search' PictureBox button
        private void pbSearch_Click(object sender, EventArgs e)
        {
            //If a search term is entered
            if (!String.IsNullOrEmpty(txtSearchTerm.Text))
            {
                pageNumber = 0;
                lblNoSearchTerm.Visible = false;

                //Display the page number
                lblPage.Text = "Page " + Convert.ToInt32(pageNumber + 1);
             
                //Invoke the search method
                performSearch();
            }
            else
            {
                //Label that informs the user that a search term is required
                lblNoSearchTerm.Visible = true;
                pbSave.Visible = false;
                pbNext.Visible = false;
                pbPrevious.Visible = false;
            }
        }

        //The Click Event for the 'Exit' PictureBox button
        private void pbExit_Click(object sender, EventArgs e)
        {
            //Closes the window and releases the associated resources
            this.Close();
            this.Dispose();
        }

        //Display tooltip on mouse hover of the 'Search' PictureBox button
        private void pbSearch_MouseHover(object sender, EventArgs e)
        {
            ToolTip tt = new ToolTip();
            tt.SetToolTip(this.pbSearch, "Search! (Return)");
        }

        //Display tooltip on mouse hover of the 'Exit' PictureBox button
        private void pbExit_MouseHover(object sender, EventArgs e)
        {
            ToolTip tt = new ToolTip();
            tt.SetToolTip(this.pbExit, "Exit (Esc)");
        }

        //Display tooltip on mouse hover of the 'Previous' PictureBox button
        private void pbPrevious_MouseHover(object sender, EventArgs e)
        {
            ToolTip tt = new ToolTip();
            tt.SetToolTip(this.pbPrevious, "Previous Page (Left Arrow)");
        }

        //Display tooltip on mouse hover of the 'Next' PictureBox button
        private void pbNext_MouseHover(object sender, EventArgs e)
        {
            ToolTip tt = new ToolTip();
            tt.SetToolTip(this.pbNext, "Next Page (Right Arrow)");
        }

        //Display tooltip on mouse hover of the 'Save' PictureBox button
        private void pbSave_MouseHover(object sender, EventArgs e)
        {
            ToolTip tt = new ToolTip();
            tt.SetToolTip(this.pbSave, "Save Images (Ctrl+S)");
        }
    }
}

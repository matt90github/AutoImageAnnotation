//Defining the class libraries
using System;
using System.Windows.Forms;
using System.Net;

namespace ImageExtractor
{
    //A form displaying the site image following successful loading
    public partial class ImageDetailForm : Form
    {
        //Default Class Contructor
        public ImageDetailForm()
        {
            InitializeComponent();
        }

        //Overloaded Constructor taking the full image URL as input
        public ImageDetailForm(string fullImageURL)
        {
            InitializeComponent();

            try {
                //Create a new PictureBox
                PictureBox pb = new PictureBox();

                //Load the image
                pb.Load(fullImageURL);

                //Set several other attributes
                pb.Left = 30;
                pb.Top = 30;
                pb.Width = this.Width - 75;
                pb.Height = this.Height - 95;
                pb.Visible = true;
                pb.BorderStyle = BorderStyle.FixedSingle;
                pb.SizeMode = PictureBoxSizeMode.StretchImage;

                //Add the PictureBox to the Form's controls
                this.Controls.Add(pb);
            }

            catch(ArgumentException)
            {
                //Close the window and release the associated resources
                this.Close();
                this.Dispose();
            }

            catch (WebException)
            {
                //Close the window and release the associated resources
                this.Close();
                this.Dispose();
            }
        }

        //Handle the Esc key stroke to 
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                //Close the window and release the associated resources
                this.Close();
                this.Dispose();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}

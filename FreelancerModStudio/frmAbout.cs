using System;
using System.Drawing;
using System.Windows.Forms;
using FreelancerModStudio.Properties;

namespace FreelancerModStudio
{
    internal partial class frmAbout : Form
    {
        public frmAbout()
        {
            InitializeComponent();
            Helper.UI.ApplyFont(this);
            FixLayout();

            Text = string.Format(Strings.AboutText, Helper.Assembly.Name);
            lblProductName.Text = "FreelancerModStudio";
            lblVersion.Text = string.Format(Strings.AboutVersion, Helper.Assembly.Version);
            lblCopyright.Text = "Copyright © stfx 2009 - 2013;\r\n" +
                                "Copyright © FreelancerAftermath 2020;\r\n" +
                                "Copyright © DiscoveryGC 2024;\r\n" +
                                "Copyright © Groshlancer 2026";
            lblCompanyName.Text = string.Empty;
            txtDescription.Text = "Vibecoded with dedication to the Freelancer community";
        }

        void FixLayout()
        {
            ClientSize = new Size(480, 320);

            logoPictureBox.SetBounds(0, 0, 165, 320);
            panel1.SetBounds(165, 0, 315, 92);

            lblProductName.SetBounds(190, 35, 270, 30);
            lblVersion.SetBounds(190, 112, 270, 18);
            lblCopyright.SetBounds(190, 132, 270, 78);
            lblCompanyName.Visible = false;

            txtDescription.SetBounds(190, 240, 270, 36);
            btnOK.SetBounds(380, 285, 75, 23);
        }
    }
}

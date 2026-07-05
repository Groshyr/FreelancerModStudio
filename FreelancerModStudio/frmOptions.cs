using System.Windows.Forms;

namespace FreelancerModStudio
{
    public partial class frmOptions : Form
    {
        public frmOptions()
        {
            InitializeComponent();
            Helper.UI.ApplyFont(this);
            propertyGrid.SelectedObject = Helper.Settings.Data.Data.General;
            propertyGrid.ExpandAllGridItems();
        }
    }
}

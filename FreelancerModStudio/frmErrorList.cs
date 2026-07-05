namespace FreelancerModStudio
{
    public partial class frmErrorList : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public frmErrorList()
        {
            InitializeComponent();
            Helper.UI.ApplyFont(this);
            Icon = Properties.Resources.Error;
        }

        public void RefreshSettings()
        {
            TabText = Properties.Strings.ErrorListText;
        }
    }
}

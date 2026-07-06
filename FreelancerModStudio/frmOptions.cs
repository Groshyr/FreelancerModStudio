using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using FreelancerModStudio.Data;

namespace FreelancerModStudio
{
    public partial class frmOptions : Form
    {
        public frmOptions()
        {
            InitializeComponent();
            Helper.UI.ApplyFont(this);
            propertyGrid.SelectedObject = new EditorOptions(Helper.Settings.Data.Data.General);
            propertyGrid.ExpandAllGridItems();
        }

        class EditorOptions
        {
            readonly Settings.General _settings;

            public EditorOptions(Settings.General settings)
            {
                _settings = settings;
            }

            [Category("Properties")]
            [DisplayName("Sort type")]
            public PropertySort PropertiesSortType
            {
                get { return _settings.PropertiesSortType; }
                set { _settings.PropertiesSortType = value; }
            }

            [Category("Properties")]
            [DisplayName("Show description")]
            public bool PropertiesShowHelp
            {
                get { return _settings.PropertiesShowHelp; }
                set { _settings.PropertiesShowHelp = value; }
            }

            [Category("INI Editor")]
            [DisplayName("Added row color")]
            public Color EditorModifiedAddedColor
            {
                get { return _settings.EditorModifiedAddedColor; }
                set { _settings.EditorModifiedAddedColor = value; }
            }

            [Category("INI Editor")]
            [DisplayName("Modified row color")]
            public Color EditorModifiedColor
            {
                get { return _settings.EditorModifiedColor; }
                set { _settings.EditorModifiedColor = value; }
            }

            [Category("INI Editor")]
            [DisplayName("Saved row color")]
            public Color EditorModifiedSavedColor
            {
                get { return _settings.EditorModifiedSavedColor; }
                set { _settings.EditorModifiedSavedColor = value; }
            }

            [Category("INI Editor")]
            [DisplayName("Hidden text color")]
            public Color EditorHiddenColor
            {
                get { return _settings.EditorHiddenColor; }
                set { _settings.EditorHiddenColor = value; }
            }

        }
    }
}

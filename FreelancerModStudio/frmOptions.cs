using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using FreelancerModStudio.Data;

namespace FreelancerModStudio
{
    public partial class frmOptions : Form
    {
        public frmOptions()
            : this(true)
        {
        }

        public frmOptions(bool iniColorsOnly)
            : this(new IniColorOptions(Helper.Settings.Data.Data.General), "INI Colors")
        {
        }

        frmOptions(object selectedObject, string title)
        {
            InitializeComponent();
            Helper.UI.ApplyFont(this);
            Text = title;
            propertyGrid.SelectedObject = selectedObject;
            propertyGrid.ExpandAllGridItems();
        }

        class IniColorOptions
        {
            readonly Settings.General _settings;

            public IniColorOptions(Settings.General settings)
            {
                _settings = settings;
            }

            [Category("INI Colors")]
            [DisplayName("Added row color")]
            public Color EditorModifiedAddedColor
            {
                get { return _settings.EditorModifiedAddedColor; }
                set { _settings.EditorModifiedAddedColor = value; }
            }

            [Category("INI Colors")]
            [DisplayName("Modified row color")]
            public Color EditorModifiedColor
            {
                get { return _settings.EditorModifiedColor; }
                set { _settings.EditorModifiedColor = value; }
            }

            [Category("INI Colors")]
            [DisplayName("Saved row color")]
            public Color EditorModifiedSavedColor
            {
                get { return _settings.EditorModifiedSavedColor; }
                set { _settings.EditorModifiedSavedColor = value; }
            }

            [Category("INI Colors")]
            [DisplayName("Hidden text color")]
            public Color EditorHiddenColor
            {
                get { return _settings.EditorHiddenColor; }
                set { _settings.EditorHiddenColor = value; }
            }
        }
    }
}

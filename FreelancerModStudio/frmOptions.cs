using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using FreelancerModStudio.Data;

namespace FreelancerModStudio
{
    public partial class frmOptions : Form
    {
        public frmOptions()
            : this(new GeneralOptions(Helper.Settings.Data.Data.General), "Options")
        {
        }

        public frmOptions(bool iniColorsOnly)
            : this(iniColorsOnly ? (object)new IniColorOptions(Helper.Settings.Data.Data.General) : new GeneralOptions(Helper.Settings.Data.Data.General),
                iniColorsOnly ? "INI Colors" : "Options")
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

        class GeneralOptions
        {
            readonly Settings.General _settings;

            public GeneralOptions(Settings.General settings)
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

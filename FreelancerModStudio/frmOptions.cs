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

            [Category("INI Editor")]
            [DisplayName("Automatically Open 3D Editor")]
            public bool AutomaticallyOpen3DEditor
            {
                get { return _settings.AutomaticallyOpen3DEditor; }
                set { _settings.AutomaticallyOpen3DEditor = value; }
            }

            [Category("INI Formatting")]
            [DisplayName("Spaces around equal sign")]
            public bool FormattingSpaces
            {
                get { return _settings.FormattingSpaces; }
                set { _settings.FormattingSpaces = value; }
            }

            [Category("INI Formatting")]
            [DisplayName("Empty line between sections")]
            public bool FormattingEmptyLine
            {
                get { return _settings.FormattingEmptyLine; }
                set { _settings.FormattingEmptyLine = value; }
            }

            [Category("INI Formatting")]
            [DisplayName("Comments")]
            public bool FormattingComments
            {
                get { return _settings.FormattingComments; }
                set { _settings.FormattingComments = value; }
            }
        }
    }
}

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
            propertyGrid.PropertySort = PropertySort.Categorized;
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

            [Category("3D View Colors")]
            [DisplayName("Construct")]
            public Color Construct { get { return _settings.ColorBox.Construct; } set { _settings.ColorBox.Construct = value; } }
            [Category("3D View Colors")]
            [DisplayName("Depot")]
            public Color Depot { get { return _settings.ColorBox.Depot; } set { _settings.ColorBox.Depot = value; } }
            [Category("3D View Colors")]
            [DisplayName("Docking ring")]
            public Color DockingRing { get { return _settings.ColorBox.DockingRing; } set { _settings.ColorBox.DockingRing = value; } }
            [Category("3D View Colors")]
            [DisplayName("Jump gate")]
            public Color JumpGate { get { return _settings.ColorBox.JumpGate; } set { _settings.ColorBox.JumpGate = value; } }
            [Category("3D View Colors")]
            [DisplayName("Jump hole")]
            public Color JumpHole { get { return _settings.ColorBox.JumpHole; } set { _settings.ColorBox.JumpHole = value; } }
            [Category("3D View Colors")]
            [DisplayName("Planet")]
            public Color Planet { get { return _settings.ColorBox.Planet; } set { _settings.ColorBox.Planet = value; } }
            [Category("3D View Colors")]
            [DisplayName("Satellite")]
            public Color Satellite { get { return _settings.ColorBox.Satellite; } set { _settings.ColorBox.Satellite = value; } }
            [Category("3D View Colors")]
            [DisplayName("Ship")]
            public Color Ship { get { return _settings.ColorBox.Ship; } set { _settings.ColorBox.Ship = value; } }
            [Category("3D View Colors")]
            [DisplayName("Station")]
            public Color Station { get { return _settings.ColorBox.Station; } set { _settings.ColorBox.Station = value; } }
            [Category("3D View Colors")]
            [DisplayName("Sun")]
            public Color Sun { get { return _settings.ColorBox.Sun; } set { _settings.ColorBox.Sun = value; } }
            [Category("3D View Colors")]
            [DisplayName("Trade lane")]
            public Color TradeLane { get { return _settings.ColorBox.TradeLane; } set { _settings.ColorBox.TradeLane = value; } }
            [Category("3D View Colors")]
            [DisplayName("Weapons platform")]
            public Color WeaponsPlatform { get { return _settings.ColorBox.WeaponsPlatform; } set { _settings.ColorBox.WeaponsPlatform = value; } }
            [Category("3D View Colors")]
            [DisplayName("Mission vignette")]
            public Color ZoneVignette { get { return _settings.ColorBox.ZoneVignette; } set { _settings.ColorBox.ZoneVignette = value; } }
            [Category("3D View Colors")]
            [DisplayName("Trade path")]
            public Color ZonePathTrade { get { return _settings.ColorBox.ZonePathTrade; } set { _settings.ColorBox.ZonePathTrade = value; } }
            [Category("3D View Colors")]
            [DisplayName("Trade-lane path")]
            public Color ZonePathTradeLane { get { return _settings.ColorBox.ZonePathTradeLane; } set { _settings.ColorBox.ZonePathTradeLane = value; } }
        }
    }
}

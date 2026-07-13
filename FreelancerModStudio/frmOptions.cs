using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
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

            Panel opacityPanel = new Panel { Dock = DockStyle.Bottom, Height = 48 };
            Label opacityLabel = new Label { Text = "3D overlay transparency", Dock = DockStyle.Top, AutoSize = true };
            TrackBar opacitySlider = new TrackBar { Dock = DockStyle.Bottom, Minimum = 5, Maximum = 90, TickFrequency = 5, Value = Helper.Settings.Data.Data.General.OverlayOpacity * 100 / 255 };
            opacitySlider.ValueChanged += (sender, args) => Helper.Settings.Data.Data.General.OverlayOpacity = (byte)(opacitySlider.Value * 255 / 100);
            opacityPanel.Controls.Add(opacitySlider);
            opacityPanel.Controls.Add(opacityLabel);
            Controls.Add(opacityPanel);
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

            static string ToWeb(Color color)
            {
                return ColorTranslator.ToHtml(color);
            }

            static bool TrySetWebColor(string value, out Color color)
            {
                color = Color.Empty;
                if (string.IsNullOrEmpty(value))
                    return false;
                try
                {
                    Color parsed = ColorTranslator.FromHtml(value.Trim());
                    if (parsed.IsSystemColor)
                        return false;
                    color = parsed;
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            void SetWebColor(string value, Action<Color> set)
            {
                Color color;
                if (TrySetWebColor(value, out color))
                    set(color);
            }

            [Category("3D View Colors")]
            [DisplayName("Construct")]
            [Editor(typeof(WebColorEditor), typeof(UITypeEditor))]
            public string Construct { get { return ToWeb(_settings.ColorBox.Construct); } set { SetWebColor(value, x => _settings.ColorBox.Construct = x); } }
            [Category("3D View Colors")]
            [DisplayName("Depot")]
            [Editor(typeof(WebColorEditor), typeof(UITypeEditor))]
            public string Depot { get { return ToWeb(_settings.ColorBox.Depot); } set { SetWebColor(value, x => _settings.ColorBox.Depot = x); } }
            [Category("3D View Colors")]
            [DisplayName("Docking ring")]
            [Editor(typeof(WebColorEditor), typeof(UITypeEditor))]
            public string DockingRing { get { return ToWeb(_settings.ColorBox.DockingRing); } set { SetWebColor(value, x => _settings.ColorBox.DockingRing = x); } }
            [Category("3D View Colors")]
            [DisplayName("Jump gate")]
            [Editor(typeof(WebColorEditor), typeof(UITypeEditor))]
            public string JumpGate { get { return ToWeb(_settings.ColorBox.JumpGate); } set { SetWebColor(value, x => _settings.ColorBox.JumpGate = x); } }
            [Category("3D View Colors")]
            [DisplayName("Jump hole")]
            [Editor(typeof(WebColorEditor), typeof(UITypeEditor))]
            public string JumpHole { get { return ToWeb(_settings.ColorBox.JumpHole); } set { SetWebColor(value, x => _settings.ColorBox.JumpHole = x); } }
            [Category("3D View Colors")]
            [DisplayName("Planet")]
            [Editor(typeof(WebColorEditor), typeof(UITypeEditor))]
            public string Planet { get { return ToWeb(_settings.ColorBox.Planet); } set { SetWebColor(value, x => _settings.ColorBox.Planet = x); } }
            [Category("3D View Colors")]
            [DisplayName("Satellite")]
            [Editor(typeof(WebColorEditor), typeof(UITypeEditor))]
            public string Satellite { get { return ToWeb(_settings.ColorBox.Satellite); } set { SetWebColor(value, x => _settings.ColorBox.Satellite = x); } }
            [Category("3D View Colors")]
            [DisplayName("Ship")]
            [Editor(typeof(WebColorEditor), typeof(UITypeEditor))]
            public string Ship { get { return ToWeb(_settings.ColorBox.Ship); } set { SetWebColor(value, x => _settings.ColorBox.Ship = x); } }
            [Category("3D View Colors")]
            [DisplayName("Station")]
            [Editor(typeof(WebColorEditor), typeof(UITypeEditor))]
            public string Station { get { return ToWeb(_settings.ColorBox.Station); } set { SetWebColor(value, x => _settings.ColorBox.Station = x); } }
            [Category("3D View Colors")]
            [DisplayName("Sun")]
            [Editor(typeof(WebColorEditor), typeof(UITypeEditor))]
            public string Sun { get { return ToWeb(_settings.ColorBox.Sun); } set { SetWebColor(value, x => _settings.ColorBox.Sun = x); } }
            [Category("3D View Colors")]
            [DisplayName("Trade lane")]
            [Editor(typeof(WebColorEditor), typeof(UITypeEditor))]
            public string TradeLane { get { return ToWeb(_settings.ColorBox.TradeLane); } set { SetWebColor(value, x => _settings.ColorBox.TradeLane = x); } }
            [Category("3D View Colors")]
            [DisplayName("Weapons platform")]
            [Editor(typeof(WebColorEditor), typeof(UITypeEditor))]
            public string WeaponsPlatform { get { return ToWeb(_settings.ColorBox.WeaponsPlatform); } set { SetWebColor(value, x => _settings.ColorBox.WeaponsPlatform = x); } }
            [Category("3D View Colors")]
            [DisplayName("Mission vignette")]
            [Editor(typeof(WebColorEditor), typeof(UITypeEditor))]
            public string ZoneVignette { get { return ToWeb(_settings.ColorBox.ZoneVignette); } set { SetWebColor(value, x => _settings.ColorBox.ZoneVignette = x); } }
            [Category("3D View Colors")]
            [DisplayName("Trade path")]
            [Editor(typeof(WebColorEditor), typeof(UITypeEditor))]
            public string ZonePathTrade { get { return ToWeb(_settings.ColorBox.ZonePathTrade); } set { SetWebColor(value, x => _settings.ColorBox.ZonePathTrade = x); } }
            [Category("3D View Colors")]
            [DisplayName("Trade-lane path")]
            [Editor(typeof(WebColorEditor), typeof(UITypeEditor))]
            public string ZonePathTradeLane { get { return ToWeb(_settings.ColorBox.ZonePathTradeLane); } set { SetWebColor(value, x => _settings.ColorBox.ZonePathTradeLane = x); } }
        }
    }

    class WebColorEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService editorService = provider == null ? null : provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            if (editorService == null)
            {
                return value;
            }

            ListBox colors = new ListBox { BorderStyle = BorderStyle.None, IntegralHeight = true, Width = 180, Height = 220 };
            List<string> colorNames = GetWebColorNames();
            for (int i = 0; i < colorNames.Count; ++i)
            {
                colors.Items.Add(colorNames[i]);
                if (value != null && colorNames[i].Equals(value.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    colors.SelectedIndex = i;
                }
            }

            colors.Click += delegate
                {
                    if (colors.SelectedItem != null)
                    {
                        editorService.CloseDropDown();
                    }
                };
            editorService.DropDownControl(colors);

            return colors.SelectedItem ?? value;
        }

        static List<string> GetWebColorNames()
        {
            List<string> names = new List<string>();
            Array values = Enum.GetValues(typeof(KnownColor));
            for (int i = 0; i < values.Length; ++i)
            {
                KnownColor knownColor = (KnownColor)values.GetValue(i);
                Color color = Color.FromKnownColor(knownColor);
                if (!color.IsSystemColor && color.A == 255)
                {
                    names.Add(knownColor.ToString());
                }
            }

            names.Sort(StringComparer.OrdinalIgnoreCase);
            return names;
        }
    }
}

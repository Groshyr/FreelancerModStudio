using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using FreelancerModStudio.SystemPresenter.Content;

namespace FreelancerModStudio.Data
{
    public class Settings
    {
        const int CURRENT_VERSION = 2;
        //const string FREELANCER_REGISTRY_KEY = "HKEY_LOCAL_MACHINE\\Software\\Microsoft\\Microsoft Games\\Freelancer\\1.0";
        //const string FREELANCER_REGISTRY_VALUE = "AppPath";

        public SettingsData Data = new SettingsData();

        public void Load(Stream stream)
        {
            Data = (SettingsData)Serializer.Load(stream, typeof(SettingsData));
        }

        public void Load(string path)
        {
            Data = (SettingsData)Serializer.Load(path, typeof(SettingsData));
        }

        public void Save(Stream stream)
        {
            Serializer.Save(stream, Data, typeof(SettingsData));
        }

        public void Save(string path)
        {
            Serializer.Save(path, Data, typeof(SettingsData));
        }

        [XmlRoot("FreelancerModStudio-Settings-1.0")]
        public class SettingsData
        {
            public General General = new General();
            public Forms Forms = new Forms();
        }

        public enum Theme
        {
            Light,
            Dark
        }

        [DisplayName("General")]
        public class General
        {
            [Browsable(false)]
            public int Version { get; set; }

            [Category("General")]
            [DisplayName("Display recent files")]
            public ushort RecentFilesCount { get; set; }

            [Category("General")]
            [DisplayName("Theme")]
            public Theme Theme { get; set; }

            [Category("Properties")]
            [DisplayName("Sort type")]
            public PropertySort PropertiesSortType { get; set; }

            [Category("Properties")]
            [DisplayName("Show description")]
            public bool PropertiesShowHelp { get; set; }

            [XmlIgnore]
            [Category("INI Editor")]
            [DisplayName("Added row color")]
            public Color EditorModifiedAddedColor { get; set; }

            [XmlIgnore]
            [Category("INI Editor")]
            [DisplayName("Modified row color")]
            public Color EditorModifiedColor { get; set; }

            [XmlIgnore]
            [Category("INI Editor")]
            [DisplayName("Saved row color")]
            public Color EditorModifiedSavedColor { get; set; }

            [XmlIgnore]
            [Category("INI Editor")]
            [DisplayName("Hidden text color")]
            public Color EditorHiddenColor { get; set; }

            [Category("INI Editor")]
            [DisplayName("Automatically Open 3D Editor")]
            public bool AutomaticallyOpen3DEditor { get; set; }

            [Browsable(false)]
            public List<ContentType> IgnoredEditorTypes { get; set; }

            [XmlElement("ColorBox")]
            public ColorBox ColorBox { get; set; }

            [Category("General")]
            [DisplayName("Restore previous files")]
            public bool RestorePreviousFiles { get; set; }

            [Browsable(false)]
            public bool ShowNavMapGrid { get; set; }

            [Category("INI Formatting")]
            [DisplayName("Spaces around equal sign")]
            public bool FormattingSpaces { get; set; }

            [Category("INI Formatting")]
            [DisplayName("Empty line between sections")]
            public bool FormattingEmptyLine { get; set; }

            [Category("INI Formatting")]
            [DisplayName("Comments")]
            public bool FormattingComments { get; set; }

            [Browsable(false)]
            public string EditorModifiedAddedColorXML
            {
                get
                {
                    return ColorTranslator.ToHtml(EditorModifiedAddedColor);
                }
                set
                {
                    EditorModifiedAddedColor = ColorTranslator.FromHtml(value);
                }
            }

            [Browsable(false)]
            public string EditorModifiedColorXML
            {
                get
                {
                    return ColorTranslator.ToHtml(EditorModifiedColor);
                }
                set
                {
                    EditorModifiedColor = ColorTranslator.FromHtml(value);
                }
            }

            [Browsable(false)]
            public string EditorModifiedSavedColorXML
            {
                get
                {
                    return ColorTranslator.ToHtml(EditorModifiedSavedColor);
                }
                set
                {
                    EditorModifiedSavedColor = ColorTranslator.FromHtml(value);
                }
            }

            [Browsable(false)]
            public string EditorHiddenColorXML
            {
                get
                {
                    return ColorTranslator.ToHtml(EditorHiddenColor);
                }
                set
                {
                    EditorHiddenColor = ColorTranslator.FromHtml(value);
                }
            }

            public AutoUpdate AutoUpdate { get; set; }

            public General()
            {
                // set default values
                RecentFilesCount = 4;
                Theme = Theme.Dark;

                PropertiesSortType = PropertySort.NoSort;
                PropertiesShowHelp = false;

                EditorModifiedAddedColor = Color.RoyalBlue;
                EditorModifiedColor = Color.MediumVioletRed;
                EditorModifiedSavedColor = Color.ForestGreen;
                EditorHiddenColor = Color.Gray;
                AutomaticallyOpen3DEditor = true;
                IgnoredEditorTypes = new List<ContentType> { ContentType.ZonePath, ContentType.ZoneVignette };
                ColorBox = new ColorBox();
                RestorePreviousFiles = true;
                ShowNavMapGrid = true;

                FormattingSpaces = true;
                FormattingEmptyLine = true;
                FormattingComments = true;

                AutoUpdate = new AutoUpdate
                    {
                        Enabled = false,
                        Proxy = new Proxy(),
                    };
                SetDefaultAutoUpdate();
            }

            public void CheckVersion()
            {
                if (IgnoredEditorTypes == null)
                    IgnoredEditorTypes = new List<ContentType> { ContentType.ZonePath, ContentType.ZoneVignette };
                if (ColorBox == null)
                    ColorBox = new ColorBox();

                if (Version < CURRENT_VERSION)
                {
                    SetDefaultAutoUpdate();
                    Version = CURRENT_VERSION;
                }
            }

            public void CheckValidData()
            {
                PropertiesSortType = PropertySort.NoSort;
                PropertiesShowHelp = false;
                AutomaticallyOpen3DEditor = true;
                FormattingSpaces = true;
                FormattingEmptyLine = true;
                FormattingComments = true;
            }

            void SetDefaultAutoUpdate()
            {
                AutoUpdate.CheckInterval = 28;
                AutoUpdate.SilentDownload = false;
                AutoUpdate.UpdateFile = @"https://github.com/Groshyr/FreelancerModStudio/releases";
            }
        }

        public class ColorBox
        {
            [XmlIgnore] public Color Construct { get; set; } = Color.Fuchsia;
            [XmlIgnore] public Color Depot { get; set; } = Color.SlateGray;
            [XmlIgnore] public Color DockingRing { get; set; } = Color.DimGray;
            [XmlIgnore] public Color JumpGate { get; set; } = Color.Green;
            [XmlIgnore] public Color JumpHole { get; set; } = Color.Firebrick;
            [XmlIgnore] public Color Planet { get; set; } = Color.FromArgb(0, 60, 120);
            [XmlIgnore] public Color Satellite { get; set; } = Color.BlueViolet;
            [XmlIgnore] public Color Ship { get; set; } = Color.Gold;
            [XmlIgnore] public Color Station { get; set; } = Color.Orange;
            [XmlIgnore] public Color Sun { get; set; } = Color.OrangeRed;
            [XmlIgnore] public Color TradeLane { get; set; } = Color.Cyan;
            [XmlIgnore] public Color WeaponsPlatform { get; set; } = Color.BurlyWood;
            [XmlIgnore] public Color ZoneVignette { get; set; } = Color.FromArgb(0, 30, 15);
            [XmlIgnore] public Color ZonePathTrade { get; set; } = Color.FromArgb(30, 0, 30);
            [XmlIgnore] public Color ZonePathTradeLane { get; set; } = Color.FromArgb(0, 30, 30);

            [Browsable(false)] public string ConstructXML { get { return ColorTranslator.ToHtml(Construct); } set { Construct = ColorTranslator.FromHtml(value); } }
            [Browsable(false)] public string DepotXML { get { return ColorTranslator.ToHtml(Depot); } set { Depot = ColorTranslator.FromHtml(value); } }
            [Browsable(false)] public string DockingRingXML { get { return ColorTranslator.ToHtml(DockingRing); } set { DockingRing = ColorTranslator.FromHtml(value); } }
            [Browsable(false)] public string JumpGateXML { get { return ColorTranslator.ToHtml(JumpGate); } set { JumpGate = ColorTranslator.FromHtml(value); } }
            [Browsable(false)] public string JumpHoleXML { get { return ColorTranslator.ToHtml(JumpHole); } set { JumpHole = ColorTranslator.FromHtml(value); } }
            [Browsable(false)] public string PlanetXML { get { return ColorTranslator.ToHtml(Planet); } set { Planet = ColorTranslator.FromHtml(value); } }
            [Browsable(false)] public string SatelliteXML { get { return ColorTranslator.ToHtml(Satellite); } set { Satellite = ColorTranslator.FromHtml(value); } }
            [Browsable(false)] public string ShipXML { get { return ColorTranslator.ToHtml(Ship); } set { Ship = ColorTranslator.FromHtml(value); } }
            [Browsable(false)] public string StationXML { get { return ColorTranslator.ToHtml(Station); } set { Station = ColorTranslator.FromHtml(value); } }
            [Browsable(false)] public string SunXML { get { return ColorTranslator.ToHtml(Sun); } set { Sun = ColorTranslator.FromHtml(value); } }
            [Browsable(false)] public string TradeLaneXML { get { return ColorTranslator.ToHtml(TradeLane); } set { TradeLane = ColorTranslator.FromHtml(value); } }
            [Browsable(false)] public string WeaponsPlatformXML { get { return ColorTranslator.ToHtml(WeaponsPlatform); } set { WeaponsPlatform = ColorTranslator.FromHtml(value); } }
            [Browsable(false)] public string ZoneVignetteXML { get { return ColorTranslator.ToHtml(ZoneVignette); } set { ZoneVignette = ColorTranslator.FromHtml(value); } }
            [Browsable(false)] public string ZonePathTradeXML { get { return ColorTranslator.ToHtml(ZonePathTrade); } set { ZonePathTrade = ColorTranslator.FromHtml(value); } }
            [Browsable(false)] public string ZonePathTradeLaneXML { get { return ColorTranslator.ToHtml(ZonePathTradeLane); } set { ZonePathTradeLane = ColorTranslator.FromHtml(value); } }
        }

        [Category("Auto Update")]
        [DisplayName("Auto Update")]
        [TypeConverter(typeof(SettingsConverter))]
        public class AutoUpdate
        {
            [DisplayName("Active")]
            public bool Enabled { get; set; }

            [DisplayName("Check each days")]
            public uint CheckInterval { get; set; }

            [DisplayName("Download silent")]
            public bool SilentDownload { get; set; }

            [DisplayName("Check file")]
            public string UpdateFile { get; set; }

            public DateTime LastCheck;

            public Update Update = new Update();
            public Proxy Proxy { get; set; }
        }

        public class Update
        {
            public string FileName;
            public bool Downloaded;
            public bool Installed;
            public bool SilentInstall;
        }

        [TypeConverter(typeof(SettingsConverter))]
        public class Proxy
        {
            [DisplayName("Active")]
            public bool Enabled { get; set; }

            [DisplayName("Address")]
            public string Uri { get; set; }

            [DisplayName("Username")]
            public string UserName { get; set; }

            public string Password { get; set; }
        }

        public class Forms
        {
            public Main Main = new Main();
            //public NewMod NewMod = new NewMod();
            public ChooseFileType ChooseFileType = new ChooseFileType();
        }

        public class Main
        {
            [XmlArrayItem("RecentFile")]
            public List<RecentFile> RecentFiles = new List<RecentFile>();

            [XmlArrayItem("OpenFile")]
            public List<RecentFile> OpenFiles = new List<RecentFile>();

            public Point Location;
            public Size Size;

            public bool Maximized;
            public bool FullScreen;
        }

        /*public class NewMod
        {
            public string ModSaveLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Mods");
            public Size Size;
        }*/

        public class ChooseFileType
        {
            public int SelectedFileType;
        }

        public class RecentFile
        {
            public string File;
            public int TemplateIndex = -1;
        }
    }

    public class SettingsConverter : ExpandableObjectConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return string.Empty;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return false;
        }
    }
}

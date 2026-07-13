using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using FreelancerModStudio.AutoUpdate;
using FreelancerModStudio.Data;
using FreelancerModStudio.Properties;
using WeifenLuo.WinFormsUI.Docking;

namespace FreelancerModStudio
{
    internal static class Helper
    {
        internal struct Program
        {
            public static void Start()
            {
#if DEBUG
                Stopwatch st = new Stopwatch();
                st.Start();
#endif
                //load settings
                Settings.Load();
                SystemPresenter.SharedGeometries.LoadColors(Settings.Data.Data.General.ColorBox);

                //install downloaded update if it exists
                if (Settings.Data.Data.General.AutoUpdate.Update.Downloaded)
                {
                    if (AutoUpdate.AutoUpdate.InstallUpdate())
                    {
                        return;
                    }
                }

                Template.Load();
#if DEBUG
                st.Stop();
                Debug.WriteLine("loading settings.xml and template.xml: " + st.ElapsedMilliseconds + "ms");
#endif

                UI.ApplyToolStripTheme();

                //remove installed update if it exists
                if (Settings.Data.Data.General.AutoUpdate.Update.Installed)
                {
                    AutoUpdate.AutoUpdate.RemoveUpdate();
                }

                //start main form
                Application.Run(new frmMain());

                //save settings
                Settings.Save();
            }
        }

        internal struct Update
        {
            public const string ReleasesUrl = "https://github.com/Groshyr/FreelancerModStudio/releases";

            public static AutoUpdate.AutoUpdate AutoUpdate = new AutoUpdate.AutoUpdate();

            public static void Check(bool silentCheck, bool silentDownload)
            {
                if (AutoUpdate.Status != StatusType.Waiting)
                {
                    AutoUpdate.ShowUI();
                    return;
                }

                Uri checkFileUri;
                if (!Uri.TryCreate(Settings.Data.Data.General.AutoUpdate.UpdateFile, UriKind.Absolute, out checkFileUri))
                {
                    if (!silentCheck)
                    {
                        MessageBox.Show(string.Format(Strings.UpdatesDownloadException, Assembly.Name), Assembly.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    Settings.Data.Data.General.AutoUpdate.LastCheck = DateTime.Now;
                    return;
                }

                AutoUpdate.CheckFileUri = checkFileUri;

                string proxy = string.Empty;
                string userName = string.Empty;
                string password = string.Empty;

                if (Settings.Data.Data.General.AutoUpdate.Proxy.Enabled)
                {
                    proxy = Settings.Data.Data.General.AutoUpdate.Proxy.Uri;
                    userName = Settings.Data.Data.General.AutoUpdate.Proxy.UserName;
                    password = Settings.Data.Data.General.AutoUpdate.Proxy.Password;
                }

                AutoUpdate.SilentCheck = silentCheck;
                AutoUpdate.SilentDownload = silentDownload;
                AutoUpdate.SetProxy(proxy);
                AutoUpdate.SetCredentials(userName, password);

                AutoUpdate.Check();
            }
        }

        internal struct UI
        {
            const string FontName = "Segoe UI";
            public static readonly Color DarkBackground = Color.FromArgb(30, 30, 30);
            public static readonly Color DarkSurface = Color.FromArgb(37, 37, 38);
            public static readonly Color DarkSurfaceAlt = Color.FromArgb(45, 45, 48);
            public static readonly Color DarkBorder = Color.FromArgb(63, 63, 70);
            public static readonly Color DarkText = Color.FromArgb(204, 204, 204);
            public static readonly Color DarkMutedText = Color.FromArgb(153, 153, 153);
            public static readonly Color DarkAccent = Color.FromArgb(0, 122, 204);

            public static bool IsDarkTheme
            {
                get
                {
                    return Settings.Data.Data.General.Theme == Data.Settings.Theme.Dark;
                }
            }

            public static Font CreateFont(Font baseFont)
            {
                if (baseFont == null)
                {
                    baseFont = SystemFonts.MessageBoxFont;
                }

                return new Font(FontName, baseFont.Size, baseFont.Style, baseFont.Unit, baseFont.GdiCharSet, baseFont.GdiVerticalFont);
            }

            public static void ApplyFont(Control control)
            {
                if (control == null)
                {
                    return;
                }

                control.Font = CreateFont(control.Font);

                foreach (Control child in control.Controls)
                {
                    ApplyFont(child);
                }
            }

            public static void ApplyToolStripTheme()
            {
                if (IsDarkTheme)
                {
                    ToolStripManager.Renderer = new DarkToolStripRenderer(new DarkColorTable());
                    return;
                }

                ProfessionalColorTable whidbeyColorTable = new ProfessionalColorTable
                    {
                        UseSystemColors = true
                    };
                ToolStripManager.Renderer = new ToolStripProfessionalRenderer(whidbeyColorTable);
            }

            public static void ApplyTheme(Control control)
            {
                if (control == null)
                {
                    return;
                }

                ApplyThemeToControl(control);

                foreach (Control child in control.Controls)
                {
                    ApplyTheme(child);
                }
            }

            public static void ApplyDockPanelTheme(DockPanel dockPanel)
            {
                if (dockPanel == null)
                {
                    return;
                }

                if (!IsDarkTheme)
                {
                    dockPanel.BackColor = SystemColors.Control;
                    dockPanel.DockBackColor = SystemColors.Control;

                    DockPanelSkin lightSkin = dockPanel.Skin;
                    SetGradient(lightSkin.AutoHideStripSkin.DockStripGradient, SystemColors.ControlLight, SystemColors.ControlLight);
                    SetTabGradient(lightSkin.AutoHideStripSkin.TabGradient, SystemColors.Control, SystemColors.Control, SystemColors.ControlDarkDark);

                    DockPaneStripGradient lightDocument = lightSkin.DockPaneStripSkin.DocumentGradient;
                    SetGradient(lightDocument.DockStripGradient, SystemColors.ControlLight, SystemColors.ControlLight);
                    SetTabGradient(lightDocument.ActiveTabGradient, SystemColors.ControlLightLight, SystemColors.ControlLightLight, SystemColors.ControlText);
                    SetTabGradient(lightDocument.InactiveTabGradient, SystemColors.ControlLight, SystemColors.ControlLight, SystemColors.ControlText);

                    DockPaneStripToolWindowGradient lightTool = lightSkin.DockPaneStripSkin.ToolWindowGradient;
                    SetGradient(lightTool.DockStripGradient, SystemColors.ControlLight, SystemColors.ControlLight);
                    SetTabGradient(lightTool.ActiveTabGradient, SystemColors.Control, SystemColors.Control, SystemColors.ControlText);
                    SetTabGradient(lightTool.InactiveTabGradient, Color.Transparent, Color.Transparent, SystemColors.ControlDarkDark);
                    SetTabGradient(lightTool.ActiveCaptionGradient, SystemColors.GradientActiveCaption, SystemColors.ActiveCaption, SystemColors.ActiveCaptionText);
                    SetTabGradient(lightTool.InactiveCaptionGradient, SystemColors.InactiveBorder, SystemColors.InactiveBorder, SystemColors.ControlText);
                    return;
                }

                dockPanel.BackColor = DarkBackground;
                dockPanel.DockBackColor = DarkBackground;

                DockPanelSkin skin = dockPanel.Skin;
                SetGradient(skin.AutoHideStripSkin.DockStripGradient, DarkSurface, DarkSurface);
                SetTabGradient(skin.AutoHideStripSkin.TabGradient, DarkSurfaceAlt, DarkSurfaceAlt, DarkText);

                DockPaneStripGradient document = skin.DockPaneStripSkin.DocumentGradient;
                SetGradient(document.DockStripGradient, DarkBackground, DarkBackground);
                SetTabGradient(document.ActiveTabGradient, DarkSurfaceAlt, DarkSurfaceAlt, Color.White);
                SetTabGradient(document.InactiveTabGradient, DarkSurface, DarkSurface, DarkMutedText);

                DockPaneStripToolWindowGradient tool = skin.DockPaneStripSkin.ToolWindowGradient;
                SetGradient(tool.DockStripGradient, DarkBackground, DarkBackground);
                SetTabGradient(tool.ActiveTabGradient, DarkSurfaceAlt, DarkSurfaceAlt, Color.White);
                SetTabGradient(tool.InactiveTabGradient, DarkSurface, DarkSurface, DarkMutedText);
                SetTabGradient(tool.ActiveCaptionGradient, DarkAccent, DarkAccent, Color.White);
                SetTabGradient(tool.InactiveCaptionGradient, DarkSurfaceAlt, DarkSurfaceAlt, DarkText);
            }

            public static void ApplyPropertyGridTheme(PropertyGrid propertyGrid)
            {
                if (propertyGrid == null)
                {
                    return;
                }

                if (!IsDarkTheme)
                {
                    propertyGrid.BackColor = SystemColors.Control;
                    propertyGrid.ViewBackColor = SystemColors.Window;
                    propertyGrid.ViewForeColor = SystemColors.WindowText;
                    propertyGrid.CategoryForeColor = SystemColors.ControlText;
                    propertyGrid.CommandsBackColor = SystemColors.Control;
                    propertyGrid.CommandsForeColor = SystemColors.ControlText;
                    propertyGrid.HelpBackColor = SystemColors.Control;
                    propertyGrid.HelpForeColor = SystemColors.ControlText;
                    propertyGrid.LineColor = SystemColors.InactiveBorder;
                    return;
                }

                propertyGrid.BackColor = DarkBackground;
                propertyGrid.ViewBackColor = DarkBackground;
                propertyGrid.ViewForeColor = DarkText;
                propertyGrid.CategoryForeColor = Color.White;
                propertyGrid.CommandsBackColor = DarkSurface;
                propertyGrid.CommandsForeColor = DarkText;
                propertyGrid.HelpBackColor = DarkSurface;
                propertyGrid.HelpForeColor = DarkText;
                propertyGrid.LineColor = DarkBorder;
            }

            static void ApplyThemeToControl(Control control)
            {
                if (!IsDarkTheme)
                {
                    if (control is TextBoxBase || control is ListView || control is TreeView || control is DataGridView || control is PropertyGrid)
                    {
                        control.BackColor = SystemColors.Window;
                        control.ForeColor = SystemColors.WindowText;
                        return;
                    }

                    if (control is Button)
                    {
                        control.BackColor = SystemColors.Control;
                        control.ForeColor = SystemColors.ControlText;
                        return;
                    }

                    if (control is ToolStrip)
                    {
                        ApplyToolStripColors((ToolStrip)control);
                        return;
                    }

                    control.BackColor = SystemColors.Control;
                    control.ForeColor = SystemColors.ControlText;
                    return;
                }

                if (control is TextBoxBase || control is ListView || control is TreeView || control is DataGridView || control is PropertyGrid)
                {
                    control.BackColor = DarkBackground;
                    control.ForeColor = DarkText;
                    return;
                }

                if (control is Button)
                {
                    control.BackColor = DarkSurfaceAlt;
                    control.ForeColor = DarkText;
                    return;
                }

                if (control is ToolStrip)
                {
                    ApplyToolStripColors((ToolStrip)control);
                    return;
                }

                control.BackColor = DarkSurface;
                control.ForeColor = DarkText;
            }

            public static void ApplyToolStripColors(ToolStrip toolStrip)
            {
                if (toolStrip == null)
                {
                    return;
                }

                toolStrip.BackColor = IsDarkTheme ? DarkSurface : SystemColors.Control;
                toolStrip.ForeColor = IsDarkTheme ? DarkText : SystemColors.ControlText;

                foreach (ToolStripItem item in toolStrip.Items)
                {
                    ApplyToolStripItemColors(item);
                }
            }

            static void ApplyToolStripItemColors(ToolStripItem item)
            {
                item.BackColor = IsDarkTheme ? DarkSurface : SystemColors.Control;
                item.ForeColor = IsDarkTheme ? DarkText : SystemColors.ControlText;

                ToolStripMenuItem menuItem = item as ToolStripMenuItem;
                if (menuItem == null)
                {
                    return;
                }

                foreach (ToolStripItem dropDownItem in menuItem.DropDownItems)
                {
                    ApplyToolStripItemColors(dropDownItem);
                }
            }

            static void SetGradient(DockPanelGradient gradient, Color startColor, Color endColor)
            {
                gradient.StartColor = startColor;
                gradient.EndColor = endColor;
            }

            static void SetTabGradient(TabGradient gradient, Color startColor, Color endColor, Color textColor)
            {
                SetGradient(gradient, startColor, endColor);
                gradient.TextColor = textColor;
            }

            sealed class DarkColorTable : ProfessionalColorTable
            {
                public override Color ToolStripDropDownBackground { get { return DarkSurface; } }
                public override Color ImageMarginGradientBegin { get { return DarkSurfaceAlt; } }
                public override Color ImageMarginGradientMiddle { get { return DarkSurfaceAlt; } }
                public override Color ImageMarginGradientEnd { get { return DarkSurfaceAlt; } }
                public override Color MenuBorder { get { return DarkBorder; } }
                public override Color MenuItemBorder { get { return DarkAccent; } }
                public override Color MenuItemSelected { get { return Color.FromArgb(62, 62, 64); } }
                public override Color MenuItemSelectedGradientBegin { get { return Color.FromArgb(62, 62, 64); } }
                public override Color MenuItemSelectedGradientEnd { get { return Color.FromArgb(62, 62, 64); } }
                public override Color MenuItemPressedGradientBegin { get { return DarkSurfaceAlt; } }
                public override Color MenuItemPressedGradientMiddle { get { return DarkSurfaceAlt; } }
                public override Color MenuItemPressedGradientEnd { get { return DarkSurfaceAlt; } }
                public override Color ToolStripBorder { get { return DarkBorder; } }
                public override Color ToolStripGradientBegin { get { return DarkSurface; } }
                public override Color ToolStripGradientMiddle { get { return DarkSurface; } }
                public override Color ToolStripGradientEnd { get { return DarkSurface; } }
                public override Color MenuStripGradientBegin { get { return DarkSurface; } }
                public override Color MenuStripGradientEnd { get { return DarkSurface; } }
                public override Color SeparatorDark { get { return DarkBorder; } }
                public override Color SeparatorLight { get { return DarkSurfaceAlt; } }
            }

            sealed class DarkToolStripRenderer : ToolStripProfessionalRenderer
            {
                public DarkToolStripRenderer(ProfessionalColorTable colorTable)
                    : base(colorTable)
                {
                }

                protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
                {
                    if (!e.Item.Enabled)
                    {
                        e.TextColor = DarkMutedText;
                    }
                    else if (e.Item.Selected || e.Item.Pressed)
                    {
                        e.TextColor = Color.White;
                    }
                    else
                    {
                        e.TextColor = DarkText;
                    }

                    base.OnRenderItemText(e);
                }
            }

        }

        internal struct Template
        {
            static FreelancerModStudio.Data.Template _data;

            public static void Load()
            {
                string file = Path.Combine(Application.StartupPath, Resources.TemplatePath);
                EnsureTemplateFile(file);
                Load(file);
            }

            public static void Load(string file)
            {
                _data = new FreelancerModStudio.Data.Template();

                try
                {
                    _data.Load(file);
                    Data.SetSpecialFiles();
                }
                catch (Exception ex)
                {
                    Exceptions.Show(string.Format(Strings.TemplateLoadException, Resources.TemplatePath), ex);
                    Environment.Exit(0);
                }
            }

            static void EnsureTemplateFile(string file)
            {
                if (File.Exists(file))
                {
                    return;
                }

                string directory = Path.GetDirectoryName(file);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                using (Stream input = assembly.GetManifestResourceStream(Resources.TemplatePath))
                {
                    if (input == null)
                    {
                        throw new FileNotFoundException("Embedded template resource was not found.", Resources.TemplatePath);
                    }

                    using (FileStream output = File.Create(file))
                    {
                        byte[] buffer = new byte[81920];
                        int read;
                        while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            output.Write(buffer, 0, read);
                        }
                    }
                }
            }

            public struct Data
            {
                public static int SystemFile { get; set; }
                public static int UniverseFile { get; set; }
                public static int SolarArchetypeFile { get; set; }
                public static int AsteroidArchetypeFile { get; set; }
                public static int ShipArchetypeFile { get; set; }
                public static int EquipmentFile { get; set; }
                public static int EffectExplosionsFile { get; set; }

                public static List<FreelancerModStudio.Data.Template.File> Files
                {
                    get
                    {
                        return _data.Data.Files;
                    }
                    set
                    {
                        _data.Data.Files = value;
                    }
                }

                //public static FreelancerModStudio.Data.Template.CostumTypes CostumTypes
                //{
                //    get { return data.Data.CostumTypes; }
                //    set { data.Data.CostumTypes = value; }
                //}

                public static int GetIndex(string file)
                {
                    FreelancerManifest manifest = FreelancerManifest.FromFile(file);
                    if (manifest != null)
                    {
                        int manifestIndex = manifest.GetTemplateIndex(file);
                        if (manifestIndex != -1)
                        {
                            return manifestIndex;
                        }
                    }

                    for (int i = 0; i < Files.Count; ++i)
                    {
                        foreach (string path in Files[i].Paths)
                        {
                            string pattern = ".*" + path.Replace("\\", "\\\\").Replace("*", "[^\\\\]*");
                            if (Regex.Match(file, pattern, RegexOptions.IgnoreCase).Success)
                            {
                                return i;
                            }
                        }
                    }
                    return -1;
                }

                public static string GetDataPath(string filePath, int fileTemplate)
                {
                    FreelancerManifest manifest = FreelancerManifest.FromFile(filePath);
                    if (manifest != null)
                    {
                        return manifest.DataPath;
                    }

                    // return if invalid file template or template path
                    if (fileTemplate < 0 ||
                        fileTemplate > Files.Count - 1 ||
                        Files[fileTemplate].Paths == null ||
                        Files[fileTemplate].Paths.Count == 0)
                    {
                        return null;
                    }

                    string[] directories = Files[fileTemplate].Paths[0].Split(new[] { Path.DirectorySeparatorChar });
                    StringBuilder builder = new StringBuilder(filePath);
                    for (int i = 0; i < directories.Length; ++i)
                    {
                        int lastIndex = builder.ToString().LastIndexOf(Path.DirectorySeparatorChar);
                        if (lastIndex == -1)
                        {
                            break;
                        }
                        builder.Remove(lastIndex, builder.Length - lastIndex);
                    }
                    return builder.ToString();
                }

                public static void SetSpecialFiles()
                {
                    int count = 7;

                    for (int i = 0; i < Files.Count && count > 0; ++i)
                    {
                        switch (Files[i].Name.ToLowerInvariant())
                        {
                            case "system":
                                SystemFile = i;
                                count--;
                                break;
                            case "universe":
                                UniverseFile = i;
                                count--;
                                break;
                            case "solar archetype":
                                SolarArchetypeFile = i;
                                count--;
                                break;
                            case "solar asteroid archetype":
                                AsteroidArchetypeFile = i;
                                count--;
                                break;
                            case "ship archetype":
                                ShipArchetypeFile = i;
                                count--;
                                break;
                            case "equipment":
                                EquipmentFile = i;
                                count--;
                                break;
                            case "effect explosions":
                                EffectExplosionsFile = i;
                                count--;
                                break;
                        }
                    }
                }
            }
        }

        internal struct Settings
        {
            public static Data.Settings Data;

            public static void Save()
            {
                string file = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Application.ProductName), Resources.SettingsPath);
                try
                {
                    string directory = Path.GetDirectoryName(file);
                    if (directory != null && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    Data.Save(file);
                }
                catch (Exception ex)
                {
                    Exceptions.Show(string.Format(Strings.SettingsSaveException, Resources.SettingsPath), ex);
                }
            }

            public static void Load()
            {
                string file = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Application.ProductName), Resources.SettingsPath);
                Data = new Data.Settings();

                if (File.Exists(file))
                {
                    try
                    {
                        Data.Load(file);
                    }
                    catch (Exception ex)
                    {
                        Exceptions.Show(string.Format(Strings.SettingsLoadException, Resources.SettingsPath), ex);
                    }
                }

                // check for valid data
                Data.Data.General.CheckVersion();
                Data.Data.General.CheckValidData();
            }

        }

        internal struct Thread
        {
            public static void Start(ref System.Threading.Thread thread, ThreadStart threadDelegate, ThreadPriority priority, bool isBackground)
            {
                Abort(ref thread, true);

                thread = new System.Threading.Thread(threadDelegate)
                    {
                        Priority = priority,
                        IsBackground = isBackground
                    };
                thread.Start();
            }

            public static void Abort(ref System.Threading.Thread thread, bool wait)
            {
                if (IsRunning(ref thread))
                {
                    thread.Abort();

                    if (wait)
                    {
                        thread.Join();
                    }
                }
            }

            public static bool IsRunning(ref System.Threading.Thread thread)
            {
                return (thread != null && thread.IsAlive);
            }
        }

        internal struct Compare
        {
            public static bool Size(Point checkSize, Point currentSize, bool bigger)
            {
                return Size(new Size(checkSize.X, checkSize.Y), new Size(currentSize.X, currentSize.Y), bigger);
            }

            public static bool Size(Size checkSize, Size currentSize, bool bigger)
            {
                if (bigger)
                {
                    return (checkSize.Width >= currentSize.Width && checkSize.Height >= currentSize.Height);
                }

                return (checkSize.Width <= currentSize.Width && checkSize.Height <= currentSize.Height);
            }
        }

        internal struct String
        {
            public static readonly StringBuilder StringBuilder = new StringBuilder();
        }

        internal struct Exceptions
        {
            public static void Show(Exception exception)
            {
                MessageBox.Show(Get(exception), Assembly.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            public static void Show(string errorDescription, Exception exception)
            {
                MessageBox.Show(errorDescription + Environment.NewLine + Environment.NewLine + Get(exception), Assembly.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            public static string Get(Exception exception)
            {
                StringBuilder stringBuilder = new StringBuilder(exception.Message);

                if (exception.InnerException != null)
                {
                    stringBuilder.Append(Environment.NewLine + Environment.NewLine + Get(exception.InnerException));
                }

                return stringBuilder.ToString();
            }
        }

        internal struct Assembly
        {
            public static string Name
            {
                get
                {
                    return Application.ProductName;
                }
            }

            public static Version Version
            {
                get
                {
                    return new Version(Application.ProductVersion);
                }
            }

            public static string Company
            {
                get
                {
                    return Application.CompanyName;
                }
            }

            public static string Description
            {
                get
                {
                    object[] attributes = System.Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                    if (attributes.Length == 0)
                    {
                        return string.Empty;
                    }

                    return ((AssemblyDescriptionAttribute)attributes[0]).Description;
                }
            }

            public static string Copyright
            {
                get
                {
                    object[] attributes = System.Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                    if (attributes.Length == 0)
                    {
                        return string.Empty;
                    }

                    return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
                }
            }
        }
    }
}

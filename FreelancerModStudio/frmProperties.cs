using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using FreelancerModStudio.Controls;
using FreelancerModStudio.Data;
using FreelancerModStudio.Data.INI;
using FreelancerModStudio.Properties;
using WeifenLuo.WinFormsUI.Docking;

namespace FreelancerModStudio
{
    public partial class frmProperties : DockContent, IContentForm
    {
        public delegate void OptionsChangedType(PropertyBlock[] blocks);

        public OptionsChangedType OptionsChanged;

        readonly ToolTip parameterDescriptionToolTip;
        Control propertyGridView;
        MethodInfo getGridEntryFromOffsetMethod;
        object lastDescriptionGridEntry;

        void OnOptionsChanged(PropertyBlock[] blocks)
        {
            if (OptionsChanged != null)
            {
                OptionsChanged(blocks);
            }
        }

        public frmProperties()
        {
            InitializeComponent();
            parameterDescriptionToolTip = new ToolTip(components)
            {
                AutomaticDelay = 2000,
                AutoPopDelay = 10000,
                InitialDelay = 2000,
                ReshowDelay = 200,
                ShowAlways = true
            };
            ConfigureParameterDescriptionToolTip();
            Helper.UI.ApplyFont(this);
            Icon = Resources.Properties;
            Helper.UI.ApplyTheme(this);
            Helper.UI.ApplyPropertyGridTheme(propertyGrid);

            RefreshSettings();
        }

        public void RefreshSettings()
        {
            Helper.UI.ApplyTheme(this);
            Helper.UI.ApplyPropertyGridTheme(propertyGrid);

            TabText = Strings.PropertiesText;

            propertyGrid.PropertySort = Helper.Settings.Data.Data.General.PropertiesSortType;
            propertyGrid.HelpVisible = Helper.Settings.Data.Data.General.PropertiesShowHelp;
        }

        public void ClearData()
        {
            if (propertyGrid.SelectedObject != null)
            {
                propertyGrid.SelectedObject = null;
            }
        }

        public void ShowData(List<TableBlock> blocks, int templateIndex)
        {
            ShowData(blocks, templateIndex, null);
        }

        public void ShowData(List<TableBlock> blocks, int templateIndex, List<TableBlock> allBlocks)
        {
            if (blocks == null)
            {
                propertyGrid.SelectedObjects = null;
                return;
            }

            Dictionary<string, bool> encounterParameterStatus = BuildEncounterParameterStatus(blocks, allBlocks);
            PropertyBlock[] propertyBlocks = new PropertyBlock[blocks.Count];
            for (int i = 0; i < blocks.Count; i++)
            {
                propertyBlocks[i] = new PropertyBlock(blocks[i].Block, Helper.Template.Data.Files[templateIndex].Blocks.Values[blocks[i].Block.TemplateIndex], encounterParameterStatus);
            }

            propertyGrid.SelectedObjects = propertyBlocks;
            propertyGrid.ExpandAllGridItems();

            //ensure visibility of selected grid item
            propertyGrid.SelectedGridItem.Select();
        }

        static Dictionary<string, bool> BuildEncounterParameterStatus(List<TableBlock> selectedBlocks, List<TableBlock> allBlocks)
        {
            if (!ContainsEncounterOption(selectedBlocks))
            {
                return null;
            }

            Dictionary<string, bool> status = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
            if (allBlocks != null)
            {
                foreach (TableBlock block in allBlocks)
                {
                    if (block == null || block.Block == null || !block.Block.Name.Equals("EncounterParameters", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    string nickname = GetOptionValue(block.Block, "nickname");
                    if (!string.IsNullOrEmpty(nickname) && !status.ContainsKey(nickname))
                    {
                        status.Add(nickname, true);
                    }
                }
            }

            foreach (TableBlock block in selectedBlocks)
            {
                if (block == null || block.Block == null)
                {
                    continue;
                }

                foreach (EditorINIOption option in block.Block.Options)
                {
                    if (!option.Name.Equals("encounter", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    foreach (EditorINIEntry entry in option.Values)
                    {
                        string encounterName = GetEncounterName(entry.Value);
                        if (encounterName.Length > 0 && !status.ContainsKey(encounterName))
                        {
                            status.Add(encounterName, false);
                        }
                    }
                }
            }

            return status;
        }

        static bool ContainsEncounterOption(List<TableBlock> blocks)
        {
            if (blocks == null)
            {
                return false;
            }

            foreach (TableBlock block in blocks)
            {
                if (block == null || block.Block == null)
                {
                    continue;
                }

                foreach (EditorINIOption option in block.Block.Options)
                {
                    if (option.Name.Equals("encounter", StringComparison.OrdinalIgnoreCase) && option.Values.Count > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        static string GetOptionValue(EditorINIBlock block, string optionName)
        {
            foreach (EditorINIOption option in block.Options)
            {
                if (option.Name.Equals(optionName, StringComparison.OrdinalIgnoreCase) && option.Values.Count > 0 && option.Values[0].Value != null)
                {
                    return option.Values[0].Value.ToString().Trim();
                }
            }

            return string.Empty;
        }

        static string GetEncounterName(object value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            string text = value.ToString();
            int commaIndex = text.IndexOf(',');
            if (commaIndex != -1)
            {
                text = text.Substring(0, commaIndex);
            }

            return text.Trim();
        }

        void descriptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            propertyGrid.HelpVisible = descriptionToolStripMenuItem.Checked;
        }

        void ConfigureParameterDescriptionToolTip()
        {
            FieldInfo gridViewField = propertyGrid.GetType().GetField("_gridView", BindingFlags.Instance | BindingFlags.NonPublic);
            if (gridViewField == null)
            {
                gridViewField = propertyGrid.GetType().GetField("gridView", BindingFlags.Instance | BindingFlags.NonPublic);
            }

            propertyGridView = gridViewField != null ? gridViewField.GetValue(propertyGrid) as Control : null;
            if (propertyGridView == null)
            {
                return;
            }

            getGridEntryFromOffsetMethod = propertyGridView.GetType().GetMethod("GetGridEntryFromOffset", BindingFlags.Instance | BindingFlags.NonPublic);
            propertyGridView.MouseMove += propertyGridView_MouseMove;
            propertyGridView.MouseLeave += propertyGridView_MouseLeave;
        }

        void propertyGridView_MouseMove(object sender, MouseEventArgs e)
        {
            object gridEntry = GetGridEntryFromOffset(e.Y);
            if (ReferenceEquals(gridEntry, lastDescriptionGridEntry))
            {
                return;
            }

            lastDescriptionGridEntry = gridEntry;
            string description = GetGridEntryDescription(gridEntry);
            parameterDescriptionToolTip.SetToolTip(propertyGridView, description);
        }

        void propertyGridView_MouseLeave(object sender, EventArgs e)
        {
            lastDescriptionGridEntry = null;
            parameterDescriptionToolTip.Hide(propertyGridView ?? propertyGrid);
        }

        object GetGridEntryFromOffset(int y)
        {
            if (getGridEntryFromOffsetMethod == null || propertyGridView == null)
            {
                return null;
            }

            try
            {
                return getGridEntryFromOffsetMethod.Invoke(propertyGridView, new object[] { y });
            }
            catch (TargetInvocationException)
            {
                return null;
            }
        }

        static string GetGridEntryDescription(object gridEntry)
        {
            if (gridEntry == null)
            {
                return null;
            }

            PropertyInfo propertyDescriptionProperty = gridEntry.GetType().GetProperty("PropertyDescription", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            string description = propertyDescriptionProperty != null ? propertyDescriptionProperty.GetValue(gridEntry, null) as string : null;
            return string.IsNullOrEmpty(description) ? "No description provided yet." : description;
        }

        void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (e.ChangedItem.Value != e.OldValue)
            {
                OnOptionsChanged((PropertyBlock[])propertyGrid.SelectedObjects);
            }
        }

        #region IContentForm Member

        public bool CanDelete()
        {
            return false;
        }

        #endregion
    }
}

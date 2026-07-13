using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using FreelancerModStudio.Data;
using FreelancerModStudio.Data.INI;

namespace FreelancerModStudio.Controls
{
    public class PropertyOptionCollectionConverter : ExpandableObjectConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            PropertySubOptions propertySubOptions = value as PropertySubOptions;
            if (propertySubOptions != null)
            {
                return "[" + ((propertySubOptions).Count - 1).ToString(CultureInfo.InvariantCulture) + "]";
            }

            return string.Empty;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return false;
        }
    }

    public class PropertyBlock : PropertyOptionCollection
    {
        public PropertyBlock(EditorINIBlock block, Template.Block templateBlock)
            : this(block, templateBlock, null)
        {
        }

        public PropertyBlock(EditorINIBlock block, Template.Block templateBlock, IDictionary<string, bool> encounterParameterStatus)
        {
            foreach (EditorINIOption option in block.Options)
            {
                List.Add(new PropertyOption(option.Values, templateBlock.Options[option.TemplateIndex], option.ChildTemplateIndex != -1, encounterParameterStatus));
            }

            // show comments
            List.Add(new PropertyOption("comments", block.Comments));
        }
    }

    public class PropertyOption
    {
        public string Name;

        public object Value;

        [Browsable(false)]
        public string Category;

        [Browsable(false)]
        public string Description;

        [Browsable(false)]
        public Attribute[] Attributes;

        public PropertyOption(string name, string value)
        {
            // comments
            Name = name;
            Value = value ?? string.Empty;

            Attributes = new Attribute[]
                {
                    new EditorAttribute(typeof(MultilineStringEditor), typeof(UITypeEditor))
                };
        }

        public PropertyOption(List<EditorINIEntry> options, Template.Option templateOption, bool children)
            : this(options, templateOption, children, null)
        {
        }

        public PropertyOption(List<EditorINIEntry> options, Template.Option templateOption, bool children, IDictionary<string, bool> encounterParameterStatus)
        {
            Name = templateOption.Name;

            Category = templateOption.Category;
            Description = templateOption.Description;

            if (templateOption.Multiple)
            {
                Attributes = new Attribute[]
                    {
                        new EditorAttribute(typeof(UITypeEditor), typeof(UITypeEditor)),
                        new TypeConverterAttribute(typeof(PropertyOptionCollectionConverter))
                    };

                Value = new PropertySubOptions(templateOption.Name, options, children, encounterParameterStatus);
            }
            else
            {
                Value = options.Count > 0 ? options[0].Value : string.Empty;

                if (Name.Equals("property_flags", StringComparison.OrdinalIgnoreCase))
                {
                    Attributes = new Attribute[]
                        {
                            new TypeConverterAttribute(typeof(PropertyFlagsConverter))
                        };
                }
                else if (Name.Equals("visit", StringComparison.OrdinalIgnoreCase))
                {
                    Attributes = new Attribute[]
                        {
                            new TypeConverterAttribute(typeof(VisitConverter))
                        };
                }
                else if (Name.Equals("shape", StringComparison.OrdinalIgnoreCase))
                {
                    Attributes = new Attribute[]
                        {
                            new TypeConverterAttribute(typeof(ZoneShapeConverter))
                        };
                }
                else if (Name.Equals("vignette_type", StringComparison.OrdinalIgnoreCase))
                {
                    Attributes = new Attribute[]
                        {
                            new TypeConverterAttribute(typeof(VignetteTypeConverter))
                        };
                }
            }
        }

        public PropertyOption(string name, object option, List<object> subOptions, bool children)
            : this(name, option, subOptions, children, null)
        {
        }

        public PropertyOption(string name, object option, List<object> subOptions, bool children, bool? encounterParameterExists)
        {
            Name = name;

            if (children)
            {
                Attributes = new Attribute[]
                    {
                        new EditorAttribute(typeof(MultilineStringEditor), typeof(UITypeEditor))
                    };

                StringBuilder valueCollection = new StringBuilder();
                valueCollection.Append(option);
                if (subOptions != null)
                {
                    foreach (object subValue in subOptions)
                    {
                        if (valueCollection.Length > 0)
                        {
                            valueCollection.Append(Environment.NewLine);
                        }

                        valueCollection.Append(subValue.ToString());
                    }
                }
                Value = valueCollection.ToString();
            }
            else
            {
                Value = option;
            }

            if (encounterParameterExists.HasValue)
            {
                List<Attribute> attributes = Attributes != null ? new List<Attribute>(Attributes) : new List<Attribute>();
                attributes.Add(new EditorAttribute(typeof(EncounterParameterStatusEditor), typeof(UITypeEditor)));
                attributes.Add(new EncounterParameterStatusAttribute(encounterParameterExists.Value));
                Attributes = attributes.ToArray();
            }
        }
    }

    public class PropertySubOptions : PropertyOptionCollection
    {
        public PropertySubOptions(string optionName, List<EditorINIEntry> options, bool children)
            : this(optionName, options, children, null)
        {
        }

        public PropertySubOptions(string optionName, List<EditorINIEntry> options, bool children, IDictionary<string, bool> encounterParameterStatus)
        {
            int index = 0;
            foreach (EditorINIEntry entry in options)
            {
                bool? status = null;
                if (encounterParameterStatus != null && optionName.Equals("encounter", StringComparison.OrdinalIgnoreCase))
                {
                    string encounterName = GetEncounterName(entry.Value);
                    if (encounterName.Length > 0 && encounterParameterStatus.ContainsKey(encounterName))
                    {
                        status = encounterParameterStatus[encounterName];
                    }
                }

                List.Add(new PropertyOption(optionName + " " + (index + 1).ToString(CultureInfo.InvariantCulture), entry.Value, entry.SubOptions, children, status));
                ++index;
            }

            List.Add(new PropertyOption(optionName + " " + (index + 1).ToString(CultureInfo.InvariantCulture), string.Empty, null, children));
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
    }

    public abstract class NamedOptionConverter : StringConverter
    {
        protected abstract string[] Values { get; }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value != null)
            {
                string text = value.ToString().Trim();
                for (int i = 0; i < Values.Length; ++i)
                {
                    if (Values[i].StartsWith(text + " - ", StringComparison.Ordinal))
                    {
                        return Values[i];
                    }
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(Values);
        }
    }

    public class PropertyFlagsConverter : NamedOptionConverter
    {
        static readonly string[] OptionValues = new[]
            {
                "0 - None", "1 - Object density low", "2 - Object density medium", "4 - Object density high",
                "8 - Danger density low", "16 - Danger density medium", "32 - Danger density high", "64 - Rock",
                "128 - Debris", "256 - Ice", "512 - Lava", "1024 - Nomad", "2048 - Crystal", "4096 - Mines",
                "8192 - Badlands", "16384 - Gas pockets", "32768 - Nebula/Cloud", "65536 - Exclusion", "131072 - Exclusion"
            };

        protected override string[] Values { get { return OptionValues; } }
    }

    public class VisitConverter : NamedOptionConverter
    {
        static readonly string[] OptionValues = new[]
            {
                "0 - Not visited", "1 - Visited", "2 - Unknown", "4 - Mineable zone", "8 - Actively visited",
                "16 - Wreck", "32 - Zone", "64 - Faction", "128 - Hidden"
            };

        protected override string[] Values { get { return OptionValues; } }
    }

    public class ZoneShapeConverter : NamedOptionConverter
    {
        static readonly string[] OptionValues = new[]
            {
                "SPHERE - Sphere", "BOX - Box", "ELLIPSOID - Ellipsoid", "CYLINDER - Cylinder", "RING - Ring"
            };

        protected override string[] Values { get { return OptionValues; } }
    }

    public class VignetteTypeConverter : NamedOptionConverter
    {
        static readonly string[] OptionValues = new[]
            {
                "field - Field", "exclusion - Exclusion", "open - Open"
            };

        protected override string[] Values { get { return OptionValues; } }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class EncounterParameterStatusAttribute : Attribute
    {
        public readonly bool Exists;

        public EncounterParameterStatusAttribute(bool exists)
        {
            Exists = exists;
        }
    }

    public class EncounterParameterStatusEditor : UITypeEditor
    {
        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override void PaintValue(PaintValueEventArgs e)
        {
            EncounterParameterStatusAttribute status = null;
            if (e.Context != null && e.Context.PropertyDescriptor != null)
            {
                status = (EncounterParameterStatusAttribute)e.Context.PropertyDescriptor.Attributes[typeof(EncounterParameterStatusAttribute)];
            }

            Color color = status != null && status.Exists ? Color.ForestGreen : Color.Firebrick;
            using (Brush brush = new SolidBrush(color))
            {
                e.Graphics.FillRectangle(brush, e.Bounds);
            }

            using (Pen pen = new Pen(SystemColors.WindowText))
            {
                Rectangle border = e.Bounds;
                border.Width -= 1;
                border.Height -= 1;
                e.Graphics.DrawRectangle(pen, border);
            }
        }
    }

    public class PropertyOptionCollection : CollectionBase, ICustomTypeDescriptor
    {
        public void Add(PropertyOption value)
        {
            List.Add(value);
        }

        public void Remove(PropertyOption value)
        {
            List.Remove(value);
        }

        public PropertyOption this[int index]
        {
            get
            {
                return (PropertyOption)List[index];
            }
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public String GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public String GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            PropertyDescriptor[] properties = new PropertyDescriptor[List.Count];

            for (int i = 0; i < List.Count; ++i)
            {
                PropertyOption propertyValue = this[i];
                properties[i] = new PropertyOptionDescriptor(propertyValue, propertyValue.Attributes);
            }

            return new PropertyDescriptorCollection(properties);
        }

        public PropertyDescriptorCollection GetProperties()
        {
            return TypeDescriptor.GetProperties(this, true);
        }
    }

    public class PropertyOptionDescriptor : PropertyDescriptor
    {
        public PropertyOption PropertyOption;

        public PropertyOptionDescriptor(PropertyOption propertyValue, Attribute[] attributes)
            : base(propertyValue.Name, attributes)
        {
            PropertyOption = propertyValue;
        }

        public override bool CanResetValue(object component)
        {
            return true;
        }

        public override Type ComponentType
        {
            get
            {
                return typeof(PropertyOption);
            }
        }

        public override string DisplayName
        {
            get
            {
                return PropertyOption.Name;
            }
        }

        public override string Category
        {
            get
            {
                return PropertyOption.Category;
            }
        }

        public override string Description
        {
            get
            {
                return PropertyOption.Description;
            }
        }

        public override object GetValue(object component)
        {
            return PropertyOption.Value;
        }

        public override bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public override Type PropertyType
        {
            get
            {
                if (PropertyOption.Value != null)
                {
                    return PropertyOption.Value.GetType();
                }

                return typeof(object);
            }
        }

        public override void ResetValue(object component)
        {
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        public override void SetValue(object component, object value)
        {
            PropertyOption.Value = value;
        }
    }
}

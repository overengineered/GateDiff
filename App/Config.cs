using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GateDiff
{
    public class CustomizationSection : ConfigurationSection
    {
        private static ConfigurationPropertyCollection s_properties;
        private static ConfigurationProperty s_tools;
        private static ConfigurationProperty s_switch;

        static CustomizationSection()
        {
            s_tools = new ConfigurationProperty(
                "tools",
                typeof(Tools),
                null,
                ConfigurationPropertyOptions.None
            );

            s_switch = new ConfigurationProperty(
                "switch",
                typeof(Switch),
                null,
                ConfigurationPropertyOptions.IsRequired
            );

            s_properties = new ConfigurationPropertyCollection();
            s_properties.Add(s_tools);
            s_properties.Add(s_switch);
        }

        [ConfigurationProperty("tools")]
        public Tools Tools
        {
            get { return (Tools)base[s_tools]; }
        }

        [ConfigurationProperty("switch")]
        public Switch Rules
        {
            get { return (Switch)base[s_switch]; }
        }
    }


    [ConfigurationCollection(typeof(Executable), AddItemName = "executable", CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class Tools : ConfigurationElementCollection
    {
        private static ConfigurationPropertyCollection m_properties = new ConfigurationPropertyCollection();

        protected override ConfigurationPropertyCollection Properties
        {
            get { return m_properties; }
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }

        public Executable this[int index]
        {
            get { return (Executable)base.BaseGet(index); }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                base.BaseAdd(index, value);
            }
        }

        public new Executable this[string name]
        {
            get { return (Executable)base.BaseGet(name); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new Executable();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as Executable).Key;
        }
    }


    [ConfigurationCollection(typeof(Case), AddItemName = "case", CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class Switch : ConfigurationElementCollection
    {
        private static ConfigurationPropertyCollection s_properties = new ConfigurationPropertyCollection();
        private static ConfigurationProperty s_default;

        static Switch()
        {
            s_default = new ConfigurationProperty(
                "default",
                typeof(String),
                null,
                ConfigurationPropertyOptions.None
            );

            s_properties = new ConfigurationPropertyCollection();
            s_properties.Add(s_default);
        }

        [ConfigurationProperty("default")]
        public string Default
        {
            get { return (string)base[s_default]; }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get { return s_properties; }
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }

        public Case this[int index]
        {
            get { return (Case)base.BaseGet(index); }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                base.BaseAdd(index, value);
            }
        }

        public new Case this[string name]
        {
            get { return (Case)base.BaseGet(name); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new Case();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as Case).Extensions;
        }
    }


    public class Executable : ConfigurationElement
    {
        private static ConfigurationPropertyCollection s_properties;
        private static ConfigurationProperty s_key;
        private static ConfigurationProperty s_path;
        private static ConfigurationProperty s_swappingParameter;

        static Executable()
        {
            s_key = new ConfigurationProperty(
                "key",
                typeof(String),
                null,
                ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey
            );

            s_path = new ConfigurationProperty(
                "path",
                typeof(String),
                null,
                ConfigurationPropertyOptions.IsRequired
            );

            s_swappingParameter = new ConfigurationProperty(
                "swappingParameter",
                typeof(String),
                null,
                ConfigurationPropertyOptions.None
            );

            s_properties = new ConfigurationPropertyCollection();

            s_properties.Add(s_key);
            s_properties.Add(s_path);
            s_properties.Add(s_swappingParameter);
        }

        [ConfigurationProperty("key", IsRequired = true)]
        public string Key
        {
            get { return (string)base[s_key]; }
        }

        [ConfigurationProperty("path", IsRequired = true)]
        public string Path
        {
            get { return (string)base[s_path]; }
        }

        [ConfigurationProperty("swappingParameter")]
        public string SwappingParameter
        {
            get { return (string)base[s_swappingParameter]; }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get { return s_properties; }
        }
    }


    public class Case : ConfigurationElement
    {
        private static ConfigurationPropertyCollection s_properties;
        private static ConfigurationProperty s_extensions;
        private static ConfigurationProperty s_tool;

        static Case()
        {
            s_extensions = new ConfigurationProperty(
                "extensions",
                typeof(String),
                null,
                ConfigurationPropertyOptions.IsRequired
            );

            s_tool = new ConfigurationProperty(
                "tool",
                typeof(String),
                null,
                ConfigurationPropertyOptions.IsRequired
            );

            s_properties = new ConfigurationPropertyCollection();

            s_properties.Add(s_extensions);
            s_properties.Add(s_tool);
        }

        [ConfigurationProperty("extensions", IsRequired = true)]
        public string Extensions
        {
            get { return (string)base[s_extensions]; }
        }

        [ConfigurationProperty("tool", IsRequired = true)]
        public string Tool
        {
            get { return (string)base[s_tool]; }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get { return s_properties; }
        }
    }

}

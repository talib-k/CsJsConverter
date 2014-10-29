using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsJsConversion.Config
{
    public class CsJsConversionAssembliesSection : ConfigurationSection
    {
        public static readonly string SectionName = CsJsConversionSectionGroup.GroupName + "/assemblies";

        private static readonly ConfigurationProperty loadReferencedAssembliesCfg = new ConfigurationProperty("loadReferencedAssemblies",
                                                                                                      typeof(string),
                                                                                                      null,
                                                                                                      ConfigurationPropertyOptions.IsRequired);

        private bool loadReferencedAssembliesSet = false;
        private bool loadReferencedAssemblies = true;

        [ConfigurationProperty("loadReferencedAssemblies", IsRequired = false, DefaultValue = null)]
        public bool LoadReferencedAssemblies
        {
            get
            {
                if (loadReferencedAssembliesSet)
                {
                    return loadReferencedAssemblies;
                }
                var tempVal = this[loadReferencedAssembliesCfg];
                if (tempVal == null)
                {
                    return loadReferencedAssemblies;
                }
                return (bool)tempVal;
            }
            set
            {
                loadReferencedAssemblies = value;
                loadReferencedAssembliesSet = true;
            }
        }


        [ConfigurationProperty("", IsRequired = false, IsDefaultCollection = true)]
        public CsJsConversionReferencedAssemblyCollection List
        {
            get { return (CsJsConversionReferencedAssemblyCollection)this[""]; }
            set { this[""] = value; }
        }

        public class CsJsConversionReferencedAssemblyCollection : ConfigurationElementCollection
        {
            protected override ConfigurationElement CreateNewElement()
            {
                return new CsJsConversionAssembly();
            }

            protected override object GetElementKey(ConfigurationElement element)
            {
                return ((CsJsConversionAssembly)element).Name;
            }
        }

        public class CsJsConversionAssembly : ConfigurationElement
        {
            [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
            public string Name
            {
                get { return (string)base["name"]; }
                set { base["name"] = value; }
            }
        }
    }
}

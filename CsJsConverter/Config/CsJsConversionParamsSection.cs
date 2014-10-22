using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsJsConversion.Config
{
    public class CsJsConversionParamsSection : ConfigurationSection
    {
        public static readonly string SectionName = CsJsSectionGroup.GroupName + "/removeScriptTags";

        private static readonly ConfigurationProperty removeScriptTagsCfg = new ConfigurationProperty("removeScriptTags",
                                                                                                      typeof(string),
                                                                                                      null,
                                                                                                      ConfigurationPropertyOptions.IsRequired);

        private bool removeScriptTagsSet = false;
        private bool removeScriptTags = true;

        [ConfigurationProperty("removeScriptTags", IsRequired = true, DefaultValue = null)]
        public bool RemoveScriptTags
        {
            get { return removeScriptTagsSet ? removeScriptTags : (bool)this[removeScriptTagsCfg]; }
            set
            {
                removeScriptTags = value;
                removeScriptTagsSet = true;
            }
        }
    }
}

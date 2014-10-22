using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsJsConversion.Config
{
    public class CsJsOutputSection : ConfigurationSection
    {
        public static readonly string SectionName = CsJsSectionGroup.GroupName + "/output";

        private static readonly ConfigurationProperty outputDirectoryCfg = new ConfigurationProperty("outputDirectory",
                                                                                                      typeof(string),
                                                                                                      null,
                                                                                                      ConfigurationPropertyOptions.IsRequired);

        private bool outputDirectorySet = false;
        private string outputDirectory;

        [ConfigurationProperty("outputDirectory", IsRequired = true, DefaultValue = null)]
        public string OutputDirectory
        {
            get { return outputDirectorySet ? outputDirectory : (string)this[outputDirectoryCfg]; }
            set
            {
                outputDirectory = value;
                outputDirectorySet = true;
            }
        }
    }
}

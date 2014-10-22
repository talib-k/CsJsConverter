using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.WebPages.Razor.Configuration;

namespace CsJsConversion.Config
{
    public class CsJsSectionGroup : ConfigurationSectionGroup
    {
        public const string GroupName = "system.web.webPages.csjs";

        private bool outputSet = false;

        private CsJsOutputSection output;

        [ConfigurationProperty("output", IsRequired = false)]
        public CsJsOutputSection Output
        {
            get { return outputSet ? output : (CsJsOutputSection)Sections["output"]; }
            set
            {
                output = value;
                outputSet = true;
            }
        }

        private bool conversionSet = false;

        private CsJsConversionSectionGroup conversion;

        [ConfigurationProperty("conversion", IsRequired = false)]
        public CsJsConversionSectionGroup Conversion
        {
            get { return conversionSet ? conversion : (CsJsConversionSectionGroup)SectionGroups["conversion"]; }
            set
            {
                conversion = value;
                conversionSet = true;
            }
        }
    }
}

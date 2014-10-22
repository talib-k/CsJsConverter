using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsJsConversion.Config
{
    public class CsJsConversionSectionGroup : ConfigurationSectionGroup
    {
        public static readonly string GroupName = CsJsSectionGroup.GroupName + "/conversion";

        private bool assembliesSet = false;
        private CsJsConversionAssembliesSection assemblies;

        [ConfigurationProperty("assemblies", IsRequired = false)]
        public CsJsConversionAssembliesSection Assemblies
        {
            get { return assembliesSet ? assemblies : (CsJsConversionAssembliesSection)Sections["assemblies"]; }
            set
            {
                assemblies = value;
                assembliesSet = true;
            }
        }


        private bool paramsSet = false;
        private CsJsConversionParamsSection _params;

        [ConfigurationProperty("params", IsRequired = false)]
        public CsJsConversionParamsSection Params
        {
            get { return paramsSet ? _params : (CsJsConversionParamsSection)Sections["params"]; }
            set
            {
                _params = value;
                paramsSet = true;
            }
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsJsConversion.BaseClassMembers;

namespace CsJsConversion
{
    public abstract class JsContentHelpersBase : JsContentGeneratorBase
    {
        protected JsContentHelpersBase()
        {
            CSharp = new CSharp();
        }

        #region Properties

        public CSharp CSharp { get; private set; }

        #endregion

    }
}

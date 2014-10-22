using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsJsConversion.BaseClassMembers;

namespace CsJsConversion
{
    public abstract class JsContentGeneratorBase
    {
        private StringBuilder buffer;

        #region Code generation utils

        /// <summary>
        /// This method is required and have to be exactly as declared here.
        /// </summary>
        public abstract void Execute();

        /// <summary>
        /// This method is required and can be public but have to have exactly the same signature
        /// </summary>
        protected void Write(object value)
        {
            WriteLiteral(value);
        }

        /// <summary>
        /// This method is required and can be public but have to have exactly the same signature
        /// </summary>
        protected void WriteLiteral(object value)
        {
            buffer.Append(value);
        }

        /// <summary>
        /// This method is just to have the rendered content without call Execute.
        /// </summary>
        /// <returns>The rendered content.</returns>
        public string GetContent()
        {
            buffer = new StringBuilder(1024);
            Execute();
            return buffer.ToString();
        }

        #endregion

    }
}

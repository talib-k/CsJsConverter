using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsJsConversion.Utils
{
    public class CsJsCompilationException : Exception
    {
        private const string ErrorDescriptionPattern = "{0} error(s) during template compilation. First error text {1}";

        public CompilerErrorCollection Errors { get; private set; }
        public string SourceCode { get; private set; }

        public CsJsCompilationException(CompilerErrorCollection errors, string sourceCode)
            : base(GetErrorDescription(errors))
        {
            Errors = new CompilerErrorCollection(errors);
            SourceCode = sourceCode;
        }

        private static string GetErrorDescription(CompilerErrorCollection errors)
        {
            return string.Format(ErrorDescriptionPattern, errors.Count, errors[0].ErrorText);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Optimization;
using CsJsConversion.Config;
using CsJsConversion.FileUtils;
using CsJsConversion.Utils;

namespace CsJsConversion.CsHelpers
{
    /// <summary>
    /// Razor helper similar to @Scripts
    /// </summary>
    public static class CsJsScripts
    {
        public static IHtmlString Url(string virtualPath)
        {
            using (var fileTransform = new CsJsFileTransform())
            {
                fileTransform.RefreshConversion(virtualPath);
                return Scripts.Url(fileTransform.GetConvertedFileName(virtualPath));
            }
        }

        public static IHtmlString Render(params string[] paths)
        {
            return Scripts.Render(paths);
        }
    }
}

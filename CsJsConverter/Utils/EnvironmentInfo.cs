using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CsJsConversion.Utils
{
    /// <summary>
    /// Wrapper around methods that are specific to Web environment
    /// </summary>
    public static class EnvironmentInfo
    {
        public static string MapToPhysicalPath(string virtualPath)
        {
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Server.MapPath(virtualPath);
            }
            return virtualPath;
        }

        public static Assembly GetCurrentAssembly()
        {
            if (HttpContext.Current != null && 
                HttpContext.Current.ApplicationInstance != null)
            {
                var type = HttpContext.Current.ApplicationInstance.GetType();
                while (type != null && type.Namespace == "ASP")
                {
                    type = type.BaseType;
                }

                return type == null ? null : type.Assembly;
            }

            return Assembly.GetEntryAssembly();
        }
    }
}

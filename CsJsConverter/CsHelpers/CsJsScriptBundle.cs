using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Optimization;
using CsJsConversion.FileUtils;

namespace CsJsConversion.CsHelpers
{
    public class CsJsScriptBundle : Bundle
    {
        public CsJsScriptBundle(string virtualPath) : base(virtualPath)
        {
        }

        public CsJsScriptBundle(string virtualPath, params IBundleTransform[] transforms)
            : base(virtualPath, transforms)
        {
        }

        public override IEnumerable<FileInfo> EnumerateFiles(BundleContext context)
        {
            using (var fileTransform = new CsJsFileTransform())
            {
                foreach (var sourceFile in base.EnumerateFiles(context))
                {
                    if (fileTransform.IsFileForConversion(sourceFile.FullName, true))
                    {
                        fileTransform.RefreshConversion(sourceFile.FullName, true);
                        yield return new FileInfo(fileTransform.GetConvertedFileName(sourceFile.FullName, true));
                    }
                    else
                    {
                        yield return sourceFile;
                    }
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsJsConversion.Config;
using CsJsConversion.Utils;

namespace CsJsConversion.FileUtils
{
    /// <summary>
    /// Utility class to calculate paths to directory where generated files will be placed
    /// </summary>
    public static class OutputDirectory
    {
        private const string DefaultFilesOutput = "~/Scripts/Generated";

        public static string Calculate(Configuration configuration, string sourceDirectory, bool physical)
        {
            var tmpPath = sourceDirectory;
            if (!physical)
            {
                tmpPath = EnvironmentInfo.MapToPhysicalPath(tmpPath);
            }
            tmpPath = Path.Combine(tmpPath, "*.*");
            var outputRoot = GetOutputDirectoryFromConfig(configuration);
            if (physical)
            {
                outputRoot = EnvironmentInfo.MapToPhysicalPath(outputRoot);
            }
            var relativePath = GetRelativePath(tmpPath, Path.GetDirectoryName(configuration.FilePath));
            return Path.GetDirectoryName(Path.Combine(outputRoot, relativePath));
        }

        private static string GetOutputDirectoryFromConfig(Configuration configuration)
        {
            if (configuration != null)
            {
                var configSectionGroup = configuration.GetSectionGroup(CsJsSectionGroup.GroupName) as CsJsSectionGroup;
                if (configSectionGroup != null &&
                    configSectionGroup.Output != null &&
                    !string.IsNullOrEmpty(configSectionGroup.Output.OutputDirectory))
                {
                    return configSectionGroup.Output.OutputDirectory;
                }
            }
            return DefaultFilesOutput;
        }

        private static string GetRelativePath(string filespec, string folder)
        {
            var pathUri = new Uri(filespec);
            // Folders must end in a slash
            if (!folder.EndsWith(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture)))
            {
                folder += Path.DirectorySeparatorChar;
            }
            var folderUri = new Uri(folder);
            return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
        }
    }
}

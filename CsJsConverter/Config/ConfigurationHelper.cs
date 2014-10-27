using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsJsConversion.Config
{
    public class ConfigurationHelper
    {
        private readonly Dictionary<string, Configuration> cachedConfigurations = new Dictionary<string, Configuration>();
        private readonly List<string> createdFiles = new List<string>();
        private bool disposed;

        public Configuration ReadConfiguration(string path)
        {
            string mergedConfig = null;
            Configuration result = null;
            if (cachedConfigurations.ContainsKey(path))
            {
                return cachedConfigurations[path];
            }
            var directory = path;
            do
            {
                string configPath = Path.Combine(directory, "Web.config");
                if (File.Exists(configPath))
                {
                    mergedConfig = (mergedConfig != null) ? MergeConfigs(configPath, mergedConfig) : configPath;
                    var fileMap = new ConfigurationFileMap(mergedConfig);
                    var configuration = ConfigurationManager.OpenMappedMachineConfiguration(fileMap);
                    result = configuration;
                }
                directory = Directory.GetParent(directory).FullName;
            } while (NormalizePath(directory) != NormalizePath(AppDomain.CurrentDomain.BaseDirectory));

            cachedConfigurations.Add(path, result);
            return result;
        }

        private static string NormalizePath(string path)
        {
            return Path.GetFullPath(new Uri(path).LocalPath)
                       .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                       .ToUpperInvariant();
        }

        private string MergeConfigs(string config1, string config2)
        {
            var merger = new ConfigFileMerger(config1, config2);
            var resultPath = Path.GetTempPath() + Guid.NewGuid() + ".config";
            merger.Save(resultPath);
            createdFiles.Add(resultPath);
            return resultPath;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }
            if (disposing)
            {
                foreach (var createdFile in createdFiles)
                {
                    try
                    {
                        File.Delete(createdFile);
                    }
                    catch
                    {
                        Console.WriteLine("Exception during {0} file deletion", createdFile);
                    }

                }
            }
            disposed = true;
        }
    }
}

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

        public Configuration ReadConfiguration(string directory)
        {
            //TODO: nested configurations
            do
            {
                if (cachedConfigurations.ContainsKey(directory))
                {
                    return cachedConfigurations[directory];
                }

                string configPath = Path.Combine(directory, "Web.config");
                if (File.Exists(configPath))
                {
                    var fileMap = new ConfigurationFileMap(configPath);
                    var configuration = ConfigurationManager.OpenMappedMachineConfiguration(fileMap);
                    cachedConfigurations.Add(directory, configuration);
                    return configuration;
                }

                directory = Directory.GetParent(directory).FullName;
            } while (directory != AppDomain.CurrentDomain.BaseDirectory);
            return null;
        }
    }
}

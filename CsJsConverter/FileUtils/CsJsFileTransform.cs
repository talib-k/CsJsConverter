using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Optimization;
using CsJsConversion.Config;
using CsJsConversion.Utils;

namespace CsJsConversion.FileUtils
{
    public class CsJsFileTransform
    {        
        private const string SourceFileExtenstion = "cshtml";
        private const string SourceFilePattern = "*." + SourceFileExtenstion;
        private const string TargetFileExtension = "js";
        private const string TargetFilePattern = "*." + TargetFileExtension;

        private readonly ConfigurationHelper configurationHelper = new ConfigurationHelper();

        private static readonly ConcurrentDictionary<string, object> LockConversion = new ConcurrentDictionary<string, object>();

        public void Convert(string path, bool isPhysicalPath = false)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("path is null");
            }

            lock (LockConversion.GetOrAdd(path, new object()))
            {
                var physicalPath = !isPhysicalPath ?
                                        EnvironmentInfo.MapToPhysicalPath(path) :
                                        path;
                FileAttributes attr = File.GetAttributes(physicalPath);
                if (attr.HasFlag(FileAttributes.Directory))
                {
                    Convert(new DirectoryInfo(physicalPath));
                }
                else
                {
                    var configuration = configurationHelper.ReadConfiguration(physicalPath);
                    var outputDirectory = OutputDirectory.Calculate(configuration, Path.GetDirectoryName(physicalPath), true);
                    Convert(new FileInfo(physicalPath), configuration, outputDirectory);
                }
            }
        }

        public void RefreshConversion(string path, bool isPhysicalPath = false)
        {
            lock (LockConversion.GetOrAdd(path, new object()))
            {
                var physicalPathSource = !isPhysicalPath ?
                                            EnvironmentInfo.MapToPhysicalPath(path) :
                                            path;
                var pathTarget = GetConvertedFileName(path, isPhysicalPath);
                var physicalPathTarget = !isPhysicalPath ?
                                            EnvironmentInfo.MapToPhysicalPath(pathTarget) :
                                            pathTarget;

                DateTime lastModificationDateSource;
                DateTime lastModificationDateTarget;
                if (File.GetAttributes(physicalPathSource).HasFlag(FileAttributes.Directory))
                {
                    lastModificationDateSource = Directory.GetLastWriteTime(physicalPathSource);
                    lastModificationDateTarget = Directory.GetLastWriteTime(physicalPathTarget);
                    if (lastModificationDateSource > lastModificationDateTarget)
                    {
                        Convert(new DirectoryInfo(path));
                    }
                }
                else
                {
                    lastModificationDateSource = File.GetLastWriteTime(physicalPathSource);
                    lastModificationDateTarget = File.GetLastWriteTime(physicalPathTarget);
                    if (lastModificationDateSource > lastModificationDateTarget)
                    {
                        var configuration = configurationHelper.ReadConfiguration(physicalPathSource);
                        Convert(new FileInfo(physicalPathSource), configuration, Path.GetDirectoryName(physicalPathTarget));
                    }
                }
            }
        }

        public bool IsFileForConversion(string fileName, bool physicalPath = false)
        {
            var physicalFileName = !physicalPath ?
                                        EnvironmentInfo.MapToPhysicalPath(fileName) :
                                        fileName;

            var fileInfo = new FileInfo(physicalFileName);

            return (fileInfo.Extension.Replace(".", string.Empty) == SourceFileExtenstion);
        }

        public string GetConvertedFileName(string sourceFileName, bool physicalPath = false)
        {
            var physicalDirectory = !physicalPath ? 
                                        Path.GetDirectoryName(EnvironmentInfo.MapToPhysicalPath(sourceFileName)) :
                                        Path.GetDirectoryName(sourceFileName);
            var sourceDirectory = Path.GetDirectoryName(sourceFileName);
            var tmpConfigurationHelper = new ConfigurationHelper();
            var configuration = tmpConfigurationHelper.ReadConfiguration(physicalDirectory);
            var outputDirectory = OutputDirectory.Calculate(configuration, sourceDirectory, physicalPath);
            return GetConvertedFileName(Path.GetFileName(sourceFileName), outputDirectory);
        }

        private void Convert(DirectoryInfo directory)
        {
            var configuration = configurationHelper.ReadConfiguration(directory.FullName);
            var outputDirectory = OutputDirectory.Calculate(configuration, directory.FullName, true);

            foreach (var subdirectory in Directory.GetDirectories(directory.FullName))
            {
                Convert(new DirectoryInfo(subdirectory));
            }

            var outputDirectoryInfo = new DirectoryInfo(outputDirectory);
            foreach (var oldFile in outputDirectoryInfo.GetFiles(TargetFilePattern))
            {
                oldFile.Delete();
            }
            if (!Directory.EnumerateFileSystemEntries(outputDirectory).Any())
            {
                outputDirectoryInfo.Delete();
            }
            foreach (var file in Directory.GetFiles(directory.FullName, SourceFilePattern))
            {
                Convert(new FileInfo(file), configuration, outputDirectory);
            }
        }

        private void Convert(FileInfo sourceFile, Configuration configuration, string outputDirectory)
        {            
            Directory.CreateDirectory(outputDirectory);

            var outputPath = GetConvertedFileName(sourceFile.Name, outputDirectory);
           
            using (var reader = new StreamReader(sourceFile.FullName))
            using (var writer = File.CreateText(outputPath))
            {
                writer.Write(CsJsConverterEngine.Convert(reader.ReadToEnd(), configuration));
            }
        }

        private static string GetConvertedFileName(string sourceFileName, string outputDirectory)
        {
            var outputPath = Path.Combine(outputDirectory, sourceFileName);
            return Path.ChangeExtension(outputPath, TargetFileExtension);
        }

    }
}

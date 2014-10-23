using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Razor;
using System.Web.WebPages.Razor.Configuration;
using CsJsConversion.Config;
using Microsoft.CSharp;

namespace CsJsConversion.Utils
{
    public struct ConversionParameters
    {
        public Type BaseClass { get; set; }
        public string[] NamespacesToAdd { get; set; }
        public string[] CustomAssembliesToLoad { get; set; }
        public bool LoadReferencedAssemblies { get; set; }
        public bool RemoveScriptTags { get; set; }
        public string SourceFileExtension { get; set; }

        public static ConversionParameters CreateForFile(FileInfo sourceFile, Configuration configuration)
        {
            return new ConversionParameters()
            {
                BaseClass = GetBaseClass(configuration),
                NamespacesToAdd = GetNamespacesToAdds(configuration),
                CustomAssembliesToLoad = GetCustomAssembliesToLoad(configuration),
                LoadReferencedAssemblies = IsLoadReferencedAssembliesSet(configuration),
                RemoveScriptTags = IsRemoveScriptTagsSet(configuration),
                SourceFileExtension = sourceFile.Extension.Replace(".", string.Empty)
            };
        }

        public RazorCodeLanguage GetCodeLanguage()
        {
            return RazorCodeLanguage.GetLanguageByExtension(SourceFileExtension);
        }

        private static bool IsRemoveScriptTagsSet(Configuration configuration)
        {
            if (configuration == null)
            {
                return true;
            }
            var configSectionGroup = configuration.SectionGroups[CsJsSectionGroup.GroupName] as CsJsSectionGroup;
            if (configSectionGroup == null || configSectionGroup.Conversion == null)
            {
                return true;
            }
            return configSectionGroup.Conversion.Params.RemoveScriptTags;
        }

        private static string[] GetNamespacesToAdds(Configuration configuration)
        {
            var result = new List<string>();
            if (configuration != null)
            {
                var configSection = configuration.SectionGroups[RazorWebSectionGroup.GroupName] as RazorWebSectionGroup;
                if (configSection != null && configSection.Pages != null && configSection.Pages.Namespaces != null)
                {
                    foreach (NamespaceInfo @namespace in configSection.Pages.Namespaces)
                    {
                        result.Add(@namespace.Namespace);
                    }
                }
            }
            return result.ToArray();
        }

        private static Type GetBaseClass(Configuration configuration)
        {
            var defaultBaseClass = typeof(JsContentHelpersBase);
            if (configuration == null)
            {
                return defaultBaseClass;
            }
            var configSection = configuration.SectionGroups[RazorWebSectionGroup.GroupName] as RazorWebSectionGroup;
            if (configSection == null || configSection.Pages == null)
            {
                return defaultBaseClass;
            }
            var baseClassName = configSection.Pages.PageBaseType;
            if (string.IsNullOrEmpty(baseClassName))
            {
                return defaultBaseClass;
            }

            var typeName = new TypeName(baseClassName);
            var assembly = Assembly.Load(typeName.AssemblyName);
            var baseClass = assembly.GetType(typeName.Name);
            if (baseClass == null)
            {
                throw new ArgumentException(string.Format("Cannot locate base class {0}", baseClassName));
            }
            if (baseClass != typeof(JsContentGeneratorBase) && !baseClass.IsSubclassOf(typeof(JsContentGeneratorBase)))
            {
                throw new ArgumentException(string.Format("Base class {0} must be subclass of JsContentGeneratorBase", baseClassName));
            }
            return baseClass;
        }

        private static bool IsLoadReferencedAssembliesSet(Configuration configuration)
        {
            if (configuration == null)
            {
                return true;
            }
            var configSectionGroup = configuration.SectionGroups[CsJsSectionGroup.GroupName] as CsJsSectionGroup;
            if (configSectionGroup == null || configSectionGroup.Conversion == null)
            {
                return true;
            }
            return configSectionGroup.Conversion.Assemblies.LoadReferencedAssemblies;
        }

        private static string[] GetCustomAssembliesToLoad(Configuration configuration)
        {
            if (configuration == null)
            {
                return new string[0];
            }
            var configSectionGroup = configuration.SectionGroups[CsJsSectionGroup.GroupName] as CsJsSectionGroup;
            if (configSectionGroup == null || configSectionGroup.Conversion == null)
            {
                return new string[0];
            }
            return configSectionGroup.Conversion.Assemblies.List.Cast<CsJsConversionAssembliesSection.CsJsConversionAssembly>()
                                                                     .Select(a => a.Name)
                                                                     .ToArray();
        }

        private class TypeName
        {
            public TypeName(string name)
            {
                var index = name.LastIndexOf(',');
                if (index > 0)
                {
                    Name = name.Substring(0, index).Trim();

                    AssemblyName = new AssemblyName(name.Substring(index + 1).Trim());
                }
                else
                {
                    Name = name;
                }
            }

            public string Name { get; private set; }

            public AssemblyName AssemblyName { get; private set; }
        }
    }
}

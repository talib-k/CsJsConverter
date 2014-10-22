using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
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
    public static class CsJsConverterEngine
    {
        private const string DynamicallyGeneratedClassName = "DynamicContentTemplate";
        private const string NamespaceForDynamicClasses = "CsJsConverter.GeneratedCode";
        private const string DynamicClassFullName = NamespaceForDynamicClasses + "." + DynamicallyGeneratedClassName;

        public static string Convert(string template, Configuration configuration)
        {
            if (string.IsNullOrEmpty(template))
            {
                return string.Empty;
            }

            var baseClass = GetBaseClass(configuration);

            var language = new CSharpRazorCodeLanguage();
            var host = new RazorEngineHost(language)
            {
                DefaultBaseClass = baseClass.FullName,
                DefaultClassName = DynamicallyGeneratedClassName,
                DefaultNamespace = NamespaceForDynamicClasses,
            };
            AddImports(host, configuration);
            var engine = new RazorTemplateEngine(host);

            var tr = ReadTemplateContent(template, IsRemoveScriptTagsSet(configuration));
            var razorTemplate = engine.GenerateCode(tr);

            var compilerResults = Compile(razorTemplate.GeneratedCode,
                                          GetCustomAssembliesToLoad(configuration),
                                          IsLoadReferencedAssembliesSet(configuration));

            if (compilerResults.Errors.HasErrors)
            {
                throw new CsJsCompilationException(compilerResults.Errors, template);
            }

            var compiledAssembly = compilerResults.CompiledAssembly;
            var templateInstance = (JsContentGeneratorBase)compiledAssembly.CreateInstance(DynamicClassFullName);

            return templateInstance.GetContent();
        }

        private static StringReader ReadTemplateContent(string template, bool removeScriptTags)
        {
            const string startScriptTag = "<script>";
            const string endScriptTag = "</script>";
            var result = template;

            if(removeScriptTags &&
                result.StartsWith(startScriptTag) &&
                result.EndsWith(endScriptTag))
            {
                result = result.Remove(0, startScriptTag.Length);
                result = result.Remove(result.Length - endScriptTag.Length);
            }
            return new StringReader(result);
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

        private static CompilerResults Compile(CodeCompileUnit unitToCompile, string[] assembliesToLoad, bool loadReferencedAssemblies)
        {
            var compilerParameters = new CompilerParameters();

            var executingAssembly = EnvironmentInfo.GetCurrentAssembly();
            compilerParameters.ReferencedAssemblies.Add(executingAssembly.Location);

            if (!loadReferencedAssemblies)
            {
                compilerParameters.ReferencedAssemblies.Add(typeof (CsJsConverterEngine).Assembly.Location);
                compilerParameters.ReferencedAssemblies.Add(typeof(object).Assembly.Location);
                compilerParameters.ReferencedAssemblies.Add(typeof(HttpRequest).Assembly.Location);
                compilerParameters.ReferencedAssemblies.Add(typeof(ControllerBase).Assembly.Location);
                compilerParameters.ReferencedAssemblies.Add(typeof(Bundle).Assembly.Location);
            }
            else
            {
                foreach (var referencedAssembly in executingAssembly.GetReferencedAssemblies())
                {
                    compilerParameters.ReferencedAssemblies.Add(GetAssemblyLocation(referencedAssembly));
                }
            }
            foreach (var assembly in assembliesToLoad)
            {
                compilerParameters.ReferencedAssemblies.Add(Assembly.Load(assembly).Location);
            }

            compilerParameters.GenerateInMemory = true;

            return new CSharpCodeProvider().CompileAssemblyFromDom(compilerParameters, unitToCompile);
        }

        private static string GetAssemblyLocation(AssemblyName referencedAssembly)
        {
            try
            {
                return Assembly.ReflectionOnlyLoad(referencedAssembly.Name).Location;
            }
            catch (IOException)
            {
                //ugly hack
                return Assembly.ReflectionOnlyLoad(referencedAssembly.FullName).Location;
            }
        }

        private static void AddImports(RazorEngineHost host, Configuration configuration)
        {
            host.NamespaceImports.Add("System");
            host.NamespaceImports.Add("System.Web");
            if (configuration != null)
            {
                var configSection = configuration.SectionGroups[RazorWebSectionGroup.GroupName] as RazorWebSectionGroup;
                if (configSection != null && configSection.Pages != null && configSection.Pages.Namespaces != null)
                {
                    foreach (NamespaceInfo @namespace in configSection.Pages.Namespaces)
                    {
                        host.NamespaceImports.Add(@namespace.Namespace);
                    }
                }
            }
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

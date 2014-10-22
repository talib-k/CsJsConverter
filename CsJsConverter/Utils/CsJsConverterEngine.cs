using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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

        public static string Convert(string template, ConversionParameters configuration)
        {
            if (string.IsNullOrEmpty(template))
            {
                return string.Empty;
            }

            var baseClass = configuration.BaseClass;

            var host = new RazorEngineHost(configuration.GetCodeLanguage())
            {
                DefaultBaseClass = baseClass.FullName,
                DefaultClassName = DynamicallyGeneratedClassName,
                DefaultNamespace = NamespaceForDynamicClasses,
            };
            AddImports(host, configuration.NamespacesToAdd);
            var engine = new RazorTemplateEngine(host);

            var tr = ReadTemplateContent(template, configuration.RemoveScriptTags);
            var razorTemplate = engine.GenerateCode(tr);

            var compilerResults = Compile(razorTemplate.GeneratedCode,
                                          configuration.CustomAssembliesToLoad,
                                          configuration.LoadReferencedAssemblies);

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
            var result = template;

            if(removeScriptTags)
            {
                result = RemoveScriptTags(result);
            }
            return new StringReader(result);
        }

        private static string RemoveScriptTags(string template)
        {
            var result = string.Empty;
            var lines = Regex.Split(template, "\r\n|\r|\n");

            var openingScriptTagLine = FindOpeningScriptTag(lines);
            var closingScriptTagLine = FindClosingScriptTag(lines);

            if (openingScriptTagLine != -1 && closingScriptTagLine != -1)
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i != openingScriptTagLine && i != closingScriptTagLine)
                    {
                        result += lines[i] + Environment.NewLine;
                    }
                }
            }
            return result;
        }

        private static int FindOpeningScriptTag(string[] lines)
        {
            const string openingScriptTag = "<script>";
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (!string.IsNullOrEmpty(line) && !line.StartsWith("@"))
                {
                    return (line.Trim() == openingScriptTag) ? i : -1;
                }
            }
            return -1;
        }

        private static int FindClosingScriptTag(string[] lines)
        {
            const string closingScriptTag = "</script>";
            for (int i = lines.Length - 1; i >= 0; i--)
            {
                var line = lines[i];
                if (!string.IsNullOrEmpty(line))
                {
                    return (line.Trim() == closingScriptTag) ? i : -1;
                }
            }
            return -1;
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

        private static void AddImports(RazorEngineHost host, string[] namespacesToAdd)
        {
            host.NamespaceImports.Add("System");
            host.NamespaceImports.Add("System.Web");
            foreach (var @namespace in namespacesToAdd)
            {
                host.NamespaceImports.Add(@namespace);
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CsJsConversion;
using CsJsConversion.Utils;
using NUnit.Framework;

namespace CsJsConversionTests.Generator
{
    [TestFixture]
    public class ConverterEngineTests
    {
        [SetUp]
        public void Init()
        {
            SetEntryAssembly(Assembly.GetExecutingAssembly());
        }

        [Test]
        public void SimpleConversionTest()
        {
            var config = ConversionParameters.CreateSimple();
            const string template = "Hello World";
            var result = CsJsConverterEngine.Convert(template, config);
            Assert.AreEqual(template, result);
        }

        [Test]
        public void SimpleConversionWithCodeTest()
        {
            var config = ConversionParameters.CreateSimple();
            const string template = "Hello @DateTime.Today";
            var result = CsJsConverterEngine.Convert(template, config);
            Assert.AreEqual(string.Format("Hello {0}", DateTime.Today), result);
        }

        [Test]
        public void BaseClassTest()
        {
            var config = ConversionParameters.CreateSimple();
            config.BaseClass = typeof (TestBaseClass);
            const string template = "@(GetType().BaseType.ToString())";
            var result = CsJsConverterEngine.Convert(template, config);
            Assert.AreEqual(typeof(TestBaseClass).ToString(), result);
        }

        [Test]
        public void AddNamespacesTest()
        {
            var config = ConversionParameters.CreateSimple();
            const string template = "@Path.DirectorySeparatorChar";
            Assert.Catch(typeof(CsJsCompilationException), () => CsJsConverterEngine.Convert(template, config));
            
            config.NamespacesToAdd = new[] {"System.IO"};
            var result = CsJsConverterEngine.Convert(template, config);
            Assert.AreEqual(Convert.ToString(Path.DirectorySeparatorChar), result);
        }

        [Test]
        public void LoadReferencedAssembliesTest()
        {
            var config = ConversionParameters.CreateSimple();
            const string template = @"@typeof(NUnit.Framework.Assert)";
            var result = CsJsConverterEngine.Convert(template, config);
            Assert.AreEqual(Convert.ToString(typeof(Assert)), result);

            config.LoadReferencedAssemblies = false;
            Assert.Catch(typeof(CsJsCompilationException), () => CsJsConverterEngine.Convert(template, config));
        }

        [Test]
        public void LoadCustomAssembliesTest()
        {
            var config = ConversionParameters.CreateSimple();
            config.LoadReferencedAssemblies = false;
            config.CustomAssembliesToLoad = new[] { "nunit.framework" };
            const string template = @"@typeof(NUnit.Framework.Assert)";
            var result = CsJsConverterEngine.Convert(template, config);
            Assert.AreEqual(Convert.ToString(typeof(Assert)), result);
        }

        [Test]
        public void RemoveScriptTags()
        {
            var config = ConversionParameters.CreateSimple();
            const string expectedResult =
@"test
";

            var template =
@"<script>
test
</script>";
            var result = CsJsConverterEngine.Convert(template, config);
            Assert.AreEqual(expectedResult, result);

            template =
@"@using NUnit.Framework
<script>
test
</script>";
            result = CsJsConverterEngine.Convert(template, config);
            Assert.AreEqual(expectedResult, result);


            template =
@"dsadasdsadas
<script>
test
</script>";
            result = CsJsConverterEngine.Convert(template, config);
            Assert.AreEqual(template, result);
        }

        public abstract class TestBaseClass : JsContentGeneratorBase { }

        private static void SetEntryAssembly(Assembly assembly)
        {
            var manager = new AppDomainManager();
            FieldInfo entryAssemblyfield = manager.GetType().GetField("m_entryAssembly", BindingFlags.Instance | BindingFlags.NonPublic);
            entryAssemblyfield.SetValue(manager, assembly);

            AppDomain domain = AppDomain.CurrentDomain;
            FieldInfo domainManagerField = domain.GetType().GetField("_domainManager", BindingFlags.Instance | BindingFlags.NonPublic);
            domainManagerField.SetValue(domain, manager);
        }

    }
}

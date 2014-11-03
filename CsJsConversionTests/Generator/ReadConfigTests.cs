using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.WebPages.Razor.Configuration;
using CsJsConversion.Config;
using NUnit.Framework;

namespace CsJsConversionTests.Generator
{
    [TestFixture]
    public class ReadConfigTests
    {
        //TODO: merge
        [Test]
        public void FileNotExistsTest()
        {
            using (var helper = new ConfigurationHelper())
            {
                var configuration = helper.ReadConfiguration(@"Content\Generator\ReadConfig");
                Assert.IsNull(configuration);
            }
        }

        [Test]
        public void FileExistsTest()
        {
            using (var helper = new ConfigurationHelper())
            {
                var configuration = helper.ReadConfiguration(@"Content\Generator\ReadConfig\Simple");
                Assert.IsNotNull(configuration);
            }
        }

        [Test]
        public void MergeTest()
        {
            using (var helper = new ConfigurationHelper())
            {
                var configuration = helper.ReadConfiguration(@"Content\Generator\ReadConfig\Simple");
                Assert.IsNotNull(configuration);
                var configSection = configuration.SectionGroups[CsJsSectionGroup.GroupName] as CsJsSectionGroup;
                Assert.IsNotNull(configSection);
                var simpleSection = configSection.Output;


                configuration = helper.ReadConfiguration(@"Content\Generator\ReadConfig\Merge\Child");
                Assert.IsNotNull(configuration);
                configSection = configuration.SectionGroups[CsJsSectionGroup.GroupName] as CsJsSectionGroup;
                Assert.IsNotNull(configSection);
                var childSection = configSection.Output;

                configuration = helper.ReadConfiguration(@"Content\Generator\ReadConfig\Merge");
                Assert.IsNotNull(configuration);
                configSection = configuration.SectionGroups[CsJsSectionGroup.GroupName] as CsJsSectionGroup;
                Assert.IsNotNull(configSection);
                var parentSection = configSection.Output;
            }
        }

    }
}

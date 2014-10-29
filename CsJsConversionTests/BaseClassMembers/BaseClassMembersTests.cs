using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using CsJsConversion.BaseClassMembers;
using NUnit.Framework;

namespace CsJsConversionTests.BaseClassMembers
{
    [TestFixture]
    public class BaseClassMembersTests
    {
        enum Enum { First = 1, Second = 2, Third = 3 }

        [Test]
        public void ClassInfoFieldName()
        {
            var cSharp = new CSharp();
            var classInfo = cSharp.GetClassInfo<string>();
            var result = classInfo.GetFieldName(x => x.Length);
            Assert.IsInstanceOf(typeof(HtmlString), result);
            Assert.AreEqual("Length", result.ToString());
        }
        
        [Test]
        public void EnumInfoFieldText()
        {
            var cSharp = new CSharp();
            var enumInfo = cSharp.GetEnumInfo<Enum>();
            var result = enumInfo.GetText(Enum.First);
            Assert.IsInstanceOf(typeof(HtmlString), result);
            Assert.AreEqual("First", result.ToString());
        }

        [Test]
        public void EnumInfoFieldValue()
        {
            var cSharp = new CSharp();
            var enumInfo = cSharp.GetEnumInfo<Enum>();
            var result = enumInfo.GetValue(Enum.First);
            Assert.IsInstanceOf(typeof(HtmlString), result);
            Assert.AreEqual("1", result.ToString());
        }

        //TODO: Tests for mvc specific items
    }
}

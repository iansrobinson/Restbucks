using System;
using System.Xml.Linq;
using NUnit.Framework;
using Restbucks.MediaType.Assemblers;

namespace Tests.Restbucks.MediaType.Assemblers
{
    [TestFixture]
    public class XNullElementsExtensionsTests
    {
        [Test]
        public void ApplyNonEmptyValueOrDefaultReturnsNullIfNodeValueDoesNotExist()
        {
            var x = XElement.Parse("<link/>");
            Assert.IsNull(x.Attribute("href").ApplyNonEmptyValueOrDefault());
        }

        [Test]
        public void ApplyNonEmptyValueOrDefaultReturnsNullIfNodeValueIsEmpty()
        {
            var x = XElement.Parse(@"<link href=""""/>");
            Assert.IsNull(x.Attribute("href").ApplyNonEmptyValueOrDefault());
        }

        [Test]
        public void ApplyNonEmptyValueOrDefaultWithFunctionYieldsNullIfNodeValueDoesNotExist()
        {
            var x = XElement.Parse("<submission/>");

            var result = x.Attribute("resource").ApplyNonEmptyValueOrDefault(value => new Uri(value, UriKind.RelativeOrAbsolute));
            Assert.IsNull(result);
        }

        [Test]
        public void ApplyNonEmptyValueOrDefaultWithFunctionYieldsNullIfNodeValueIsEmpty()
        {
            var x = XElement.Parse(@"<submission resource=""""/>");

            var result = x.Attribute("resource").ApplyNonEmptyValueOrDefault(value => new Uri(value, UriKind.RelativeOrAbsolute));
            Assert.IsNull(result);
        }

        [Test]
        public void ApplyNonEmptyValueOrDefaultWithDefaultValueYieldsDefaultValueIfNodeValueDoesNotExist()
        {
            var x = XElement.Parse("<submission/>");

            var result = x.Attribute("resource").ApplyNonEmptyValueOrDefault(value => "new value", "default value");
            Assert.AreEqual("default value", result);
        }

        [Test]
        public void ApplyNonEmptyValueOrDefaultWithDefaultValueYieldsDefaultValueIfNodeValueIsEmpty()
        {
            XElement x = XElement.Parse(@"<submission resource=""""/>");

            dynamic result = x.Attribute("resource").ApplyNonEmptyValueOrDefault(value => "new value", "default value");
            Assert.AreEqual("default value", result);
        }

        [Test]
        public void ApplyNonEmptyValueOrDefaultWithFunctionAndDefaultValueEvaluatesFunctionIfNodeValueIsNotEmpty()
        {
            var x = XElement.Parse(@"<submission resource=""abc""/>");

            var result = x.Attribute("resource").ApplyNonEmptyValueOrDefault(value => value.ToUpper(), "default value");
            Assert.AreEqual("ABC", result);
        }

        [Test]
        public void ApplyNonNullInstanceAppliesFunctionIfInstanceIsNotNull()
        {
            var x = XElement.Parse(@"<submission resource=""abc""/>");

            var result = x.ApplyNonNullInstance(o => "new result");
            Assert.AreEqual("new result", result);
        }

        [Test]
        public void ApplyNonNullInstanceReturnsNullIfInstanceIsNull()
        {
            XElement x = null;

            var result = x.ApplyNonNullInstance(o => "new result");
            Assert.IsNull(result);
        }
    }
}
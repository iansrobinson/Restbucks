using System;
using System.IO;
using System.Xml;
using NUnit.Framework;
using RestInPractice.RestToolkit.Utils;

namespace Tests.RestInPractice.RestToolkit.Utils
{
    [TestFixture]
    public class XmlTransformTests
    {
        [Test]
        public void WhenTransformFunctionAppliesNoTransformShouldCopyElementAndTextContentFromInputToOutput()
        {
            const string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<root>
  <an-element>Some content</an-element>
</root>";
            const string expectedXml = xml;

            AssertApplyingTransformProducesExpectedXml(xml, expectedXml);
        }

        [Test]
        public void WhenTransformFunctionAppliesNoTransformShouldCopyElementAttributesFromInputToOutput()
        {
            const string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<root>
  <an-element an-attribute=""a value"">Some content</an-element>
</root>";
            const string expectedXml = xml;

            AssertApplyingTransformProducesExpectedXml(xml, expectedXml);
        }

        [Test]
        public void WhenTransformFunctionAppliesNoTransformShouldCopyEmptyElementFromInputToOutput()
        {
            const string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<root>
  <an-element />
</root>";
            const string expectedXml = xml;

            AssertApplyingTransformProducesExpectedXml(xml, expectedXml);
        }

        [Test]
        public void WhenTransformFunctionAppliesNoTransformShouldCopyElementNamespaceFromInputToOutput()
        {
            const string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<root>
  <an-element xmlns=""http://a.namespace"">Some content</an-element>
</root>";
            const string expectedXml = xml;

            AssertApplyingTransformProducesExpectedXml(xml, expectedXml);
        }

        [Test]
        public void WhenTransformFunctionAppliesNoTransformShouldCopyElementNamespaceWithPrefixFromInputToOutput()
        {
            const string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<root xmlns:s=""http://a.namespace"">
  <s:an-element>Some content</s:an-element>
</root>";
            const string expectedXml = xml;

            AssertApplyingTransformProducesExpectedXml(xml, expectedXml);
        }

        [Test]
        public void WhenTransformFunctionAppliesNoTransformShouldCopyCDataFromInputToOutput()
        {
            const string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<root>
  <an-element><![CDATA[Some content]]></an-element>
</root>";
            const string expectedXml = xml;

            AssertApplyingTransformProducesExpectedXml(xml, expectedXml);
        }

        [Test]
        public void WhenTransformFunctionAppliesNoTransformShouldCopyDtdFromInputToOutput()
        {
            const string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE note [<!ELEMENT root (an-element)><!ELEMENT an-element (#PCDATA)>]>
<root>
  <an-element>Some content</an-element>
</root>";
            const string expectedXml = xml;

            AssertApplyingTransformProducesExpectedXml(xml, expectedXml, s => new XmlTransform(s, new XmlReaderSettings {IgnoreWhitespace = true, DtdProcessing = DtdProcessing.Parse}));
        }

        [Test]
        public void WhenTransformFunctionAppliesNoTransformShouldCopyEntityReferenceFromInputToOutput()
        {
            const string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE note [<!ELEMENT root (an-element)><!ELEMENT an-element (#PCDATA)><!ENTITY E ""an entity"">]>
<root>
  <an-element>This content comprises &E;</an-element>
</root>";
            const string expectedXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE note [<!ELEMENT root (an-element)><!ELEMENT an-element (#PCDATA)><!ENTITY E ""an entity"">]>
<root>
  <an-element>This content comprises an entity</an-element>
</root>";

            AssertApplyingTransformProducesExpectedXml(xml, expectedXml, s => new XmlTransform(s, new XmlReaderSettings {IgnoreWhitespace = true, DtdProcessing = DtdProcessing.Parse}));
        }

        [Test]
        public void WhenTransformFunctionAppliesNoTransformShouldCopyEntityReferenceInAttributeFromInputToOutput()
        {
            const string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE note [<!ELEMENT root (an-element)><!ELEMENT an-element (#PCDATA)><!ENTITY E ""An entity"">]>
<root>
  <an-element an-attribute=""&E;"">Some content</an-element>
</root>";
            const string expectedXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE note [<!ELEMENT root (an-element)><!ELEMENT an-element (#PCDATA)><!ENTITY E ""An entity"">]>
<root>
  <an-element an-attribute=""An entity"">Some content</an-element>
</root>";

            AssertApplyingTransformProducesExpectedXml(xml, expectedXml, s => new XmlTransform(s, new XmlReaderSettings {IgnoreWhitespace = true, DtdProcessing = DtdProcessing.Parse}));
        }

        [Test]
        public void WhenTransformFunctionAppliesNoTransformShouldCopyProcessingInstructionFromInputToOutput()
        {
            const string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<?xml-stylesheet href=""headlines.css"" type=""text/css""?>
<root>
  <an-element>Some content</an-element>
</root>";
            const string expectedXml = xml;

            AssertApplyingTransformProducesExpectedXml(xml, expectedXml);
        }

        [Test]
        public void WhenTransformFunctionAppliesNoTransformShouldCopyCommentFromInputToOutput()
        {
            const string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!--A Comment-->
<root>
  <an-element>Some content</an-element>
</root>";
            const string expectedXml = xml;

            AssertApplyingTransformProducesExpectedXml(xml, expectedXml);
        }

        [Test]
        public void WhenTransformFunctionAppliesNoTransformAndXmlSettingsSpecifyIncludingWitespaceShouldCopyWhitespaceFromInputToOutput()
        {
            const string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<root>
  <an-element>Some content</an-element> <another-element>Some more content</another-element>
</root>";
            const string expectedXml = xml;

            AssertApplyingTransformProducesExpectedXml(xml, expectedXml, s => new XmlTransform(s, new XmlReaderSettings {IgnoreWhitespace = false}));
        }

        [Test]
        public void WhenTransformFunctionAppliesNoTransformShouldCopySgnificantWhitespaceFromInputToOutput()
        {
            const string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<root>
  <an-element xml:space=""preserve""> </an-element>
</root>";
            const string expectedXml = xml;

            AssertApplyingTransformProducesExpectedXml(xml, expectedXml);
        }

        [Test]
        public void ShouldDisposeInputStreamAfterTransform()
        {
            using (Stream streamIn = new MemoryStream())
            {
                using (var streamOut = new MemoryStream())
                {
                    var writer = new StreamWriter(streamIn);
                    writer.Write("<root>Some content</root>");
                    writer.Flush();

                    streamIn.Seek(0, SeekOrigin.Begin);

                    using (var transform = new XmlTransform(streamIn))
                    {
                        transform.Transform(streamOut, (r, w) => false);
                    }

                    Assert.IsFalse(streamIn.CanRead);
                    Assert.IsFalse(streamIn.CanWrite);
                }
            }
        }

        private static void AssertApplyingTransformProducesExpectedXml(string xml, string expectedXml)
        {
            AssertApplyingTransformProducesExpectedXml(xml, expectedXml, s => new XmlTransform(s));
        }

        private static void AssertApplyingTransformProducesExpectedXml(string xml, string expectedXml, Func<Stream, XmlTransform> createTransform)
        {
            using (var streamIn = new MemoryStream())
            {
                using (var streamOut = new MemoryStream())
                {
                    var writer = new StreamWriter(streamIn);
                    writer.Write(xml);
                    writer.Flush();

                    streamIn.Seek(0, SeekOrigin.Begin);

                    var transform = createTransform(streamIn);
                    transform.Transform(streamOut, (r, w) => false);

                    streamOut.Seek(0, SeekOrigin.Begin);
                    using (var reader = new StreamReader(streamOut))
                    {
                        var xmlOut = reader.ReadToEnd();
                        Assert.AreEqual(expectedXml, xmlOut);
                    }
                }
            }
        }
    }
}
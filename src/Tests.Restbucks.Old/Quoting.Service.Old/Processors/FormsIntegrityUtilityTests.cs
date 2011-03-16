using System.IO;
using NUnit.Framework;
using Restbucks.Quoting.Service.Old.Processors;
using Rhino.Mocks;

namespace Tests.Restbucks.Old.Quoting.Service.Old.Processors
{
    [TestFixture]
    public class FormsIntegrityUtilityTests
    {
        [Test]
        public void ShouldSignResourceUriWhenInstanceElementIsNotEmpty()
        {
            const string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<shop xmlns=""http://schemas.restbucks.com/shop"">
  <model xmlns=""http://www.w3.org/2002/xforms"">
    <instance>
      <shop xmlns=""http://schemas.restbucks.com/shop"">
        <status>Awaiting Payment</status>
      </shop>
    </instance>
    <submission resource=""/orders?c=123&amp;s=PLACEHOLDER"" method=""put"" mediatype=""application/xml"" />
  </model>
</shop>";

            const string expectedXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<shop xmlns=""http://schemas.restbucks.com/shop"">
  <model xmlns=""http://www.w3.org/2002/xforms"">
    <instance>
      <shop xmlns=""http://schemas.restbucks.com/shop"">
        <status>Awaiting Payment</status>
      </shop>
    </instance>
    <submission resource=""/orders?c=123&amp;s=88"" method=""put"" mediatype=""application/xml"" />
  </model>
</shop>";

            AssertProtectingFormCreatesExpectedXml(xml, expectedXml, StringLengthSignature.Instance);
        }

        [Test]
        public void ShouldSignResourceUriEvenIfInstanceElementIsEmpty()
        {
            const string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<shop xmlns=""http://schemas.restbucks.com/shop"">
  <model xmlns=""http://www.w3.org/2002/xforms"">
    <instance />
    <submission resource=""/orders?c=123&amp;s=PLACEHOLDER"" method=""put"" mediatype=""application/xml"" />
  </model>
</shop>";

            const string expectedXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<shop xmlns=""http://schemas.restbucks.com/shop"">
  <model xmlns=""http://www.w3.org/2002/xforms"">
    <instance />
    <submission resource=""/orders?c=123&amp;s=0"" method=""put"" mediatype=""application/xml"" />
  </model>
</shop>";

            AssertProtectingFormCreatesExpectedXml(xml, expectedXml, StringLengthSignature.Instance);
        }

        [Test]
        public void ShouldSignResourceUriEvenIfInstanceElementIsMissing()
        {
            const string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<shop xmlns=""http://schemas.restbucks.com/shop"">
  <model xmlns=""http://www.w3.org/2002/xforms"">
    <submission resource=""/orders?c=123&amp;s=PLACEHOLDER"" method=""put"" mediatype=""application/xml"" />
  </model>
</shop>";

            const string expectedXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<shop xmlns=""http://schemas.restbucks.com/shop"">
  <model xmlns=""http://www.w3.org/2002/xforms"">
    <submission resource=""/orders?c=123&amp;s=0"" method=""put"" mediatype=""application/xml"" />
  </model>
</shop>";

            AssertProtectingFormCreatesExpectedXml(xml, expectedXml, StringLengthSignature.Instance);
        }

        [Test]
        public void ShouldNotModifyFormContentsIfSubmissionElementIsMissing()
        {
            const string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<shop xmlns=""http://schemas.restbucks.com/shop"">
  <model xmlns=""http://www.w3.org/2002/xforms"">
    <instance>
      <shop xmlns=""http://schemas.restbucks.com/shop"">
        <status>Awaiting Payment</status>
      </shop>
    </instance>
  </model>
</shop>";

            const string expectedXml = xml;

            AssertProtectingFormCreatesExpectedXml(xml, expectedXml, StringLengthSignature.Instance);
        }

        [Test]
        public void ShouldNotSignFormIfResourceAttributeIsMissing()
        {
            const string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<shop xmlns=""http://schemas.restbucks.com/shop"">
  <model xmlns=""http://www.w3.org/2002/xforms"">
    <instance>
      <shop xmlns=""http://schemas.restbucks.com/shop"">
        <status>Awaiting Payment</status>
      </shop>
    </instance>
    <submission method=""put"" mediatype=""application/xml"" />
  </model>
</shop>";

            const string expectedXml = xml;

            AssertProtectingFormCreatesExpectedXml(xml, expectedXml, StringLengthSignature.Instance);
        }

        [Test]
        public void ShouldNotSignFormIfPlaceholderIsMissingFromResourceUri()
        {
            const string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<shop xmlns=""http://schemas.restbucks.com/shop"">
  <model xmlns=""http://www.w3.org/2002/xforms"">
    <instance>
      <shop xmlns=""http://schemas.restbucks.com/shop"">
        <status>Awaiting Payment</status>
      </shop>
    </instance>
    <submission resource=""/orders?c=123"" method=""put"" mediatype=""application/xml"" />
  </model>
</shop>";

            const string expectedXml = xml;

            AssertProtectingFormCreatesExpectedXml(xml, expectedXml, StringLengthSignature.Instance);
        }

        [Test]
        public void ShouldSignMultipleForms()
        {
            const string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<shop xmlns=""http://schemas.restbucks.com/shop"">
  <model xmlns=""http://www.w3.org/2002/xforms"">
    <instance>
      <shop xmlns=""http://schemas.restbucks.com/shop"">
        <status>Awaiting Payment</status>
      </shop>
    </instance>
    <submission resource=""/orders?c=123&amp;s=PLACEHOLDER"" method=""put"" mediatype=""application/xml"" />
  </model>
  <model xmlns=""http://www.w3.org/2002/xforms"">
    <instance>
      <shop xmlns=""http://schemas.restbucks.com/shop"">
        <status>Paid</status>
      </shop>
    </instance>
    <submission resource=""/orders?c=987&amp;s=PLACEHOLDER"" method=""put"" mediatype=""application/xml"" />
  </model>
</shop>";

            const string expectedXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<shop xmlns=""http://schemas.restbucks.com/shop"">
  <model xmlns=""http://www.w3.org/2002/xforms"">
    <instance>
      <shop xmlns=""http://schemas.restbucks.com/shop"">
        <status>Awaiting Payment</status>
      </shop>
    </instance>
    <submission resource=""/orders?c=123&amp;s=88"" method=""put"" mediatype=""application/xml"" />
  </model>
  <model xmlns=""http://www.w3.org/2002/xforms"">
    <instance>
      <shop xmlns=""http://schemas.restbucks.com/shop"">
        <status>Paid</status>
      </shop>
    </instance>
    <submission resource=""/orders?c=987&amp;s=76"" method=""put"" mediatype=""application/xml"" />
  </model>
</shop>";

            AssertProtectingFormCreatesExpectedXml(xml, expectedXml, StringLengthSignature.Instance);
        }

        [Test]
        public void ShouldNotCarryOverSignatureFromOneFormToAnother()
        {
            const string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<shop xmlns=""http://schemas.restbucks.com/shop"">
  <model xmlns=""http://www.w3.org/2002/xforms"">
    <instance>
      <shop xmlns=""http://schemas.restbucks.com/shop"">
        <status>Awaiting Payment</status>
      </shop>
    </instance>
  </model>
  <model xmlns=""http://www.w3.org/2002/xforms"">
    <instance>
      <shop xmlns=""http://schemas.restbucks.com/shop"">
        <status>Paid</status>
      </shop>
    </instance>
    <submission resource=""/orders?c=987&amp;s=PLACEHOLDER"" method=""put"" mediatype=""application/xml"" />
  </model>
</shop>";

            const string expectedXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<shop xmlns=""http://schemas.restbucks.com/shop"">
  <model xmlns=""http://www.w3.org/2002/xforms"">
    <instance>
      <shop xmlns=""http://schemas.restbucks.com/shop"">
        <status>Awaiting Payment</status>
      </shop>
    </instance>
  </model>
  <model xmlns=""http://www.w3.org/2002/xforms"">
    <instance>
      <shop xmlns=""http://schemas.restbucks.com/shop"">
        <status>Paid</status>
      </shop>
    </instance>
    <submission resource=""/orders?c=987&amp;s=76"" method=""put"" mediatype=""application/xml"" />
  </model>
</shop>";

            AssertProtectingFormCreatesExpectedXml(xml, expectedXml, StringLengthSignature.Instance);
        }

        [Test]
        public void ShouldOmitExtraneousWhitespaceWhenPassingXmlToSignatureGenerator()
        {
            const string originalXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<shop xmlns=""http://schemas.restbucks.com/shop"">
  <model xmlns=""http://www.w3.org/2002/xforms"">
    <instance>
      <shop xmlns=""http://schemas.restbucks.com/shop"">
        <status>Awaiting Payment</status>
      </shop>
    </instance>
    <submission resource=""/orders?c=123&amp;s=PLACEHOLDER"" method=""put"" mediatype=""application/xml"" />
  </model>
</shop>";
            const string expectedXml = @"<shop xmlns=""http://schemas.restbucks.com/shop""><status>Awaiting Payment</status></shop>";

            var mockSignature = MockRepository.GenerateMock<IGenerateSignature>();

            mockSignature.Expect(s => s.GenerateSignature(expectedXml)).Return("signature");

            using (var streamIn = new MemoryStream())
            {
                using (var streamOut = new MemoryStream())
                {
                    var writer = new StreamWriter(streamIn);
                    writer.Write(originalXml);
                    writer.Flush();

                    var formProtection = new FormsIntegrityUtility(mockSignature, "PLACEHOLDER");
                    formProtection.SignForms(streamIn, streamOut);
                }
            }

            mockSignature.VerifyAllExpectations();
        }

        private static void AssertProtectingFormCreatesExpectedXml(string originalXml, string expectedXml, IGenerateSignature signature)
        {
            using (var streamIn = new MemoryStream())
            {
                using (var streamOut = new MemoryStream())
                {
                    var writer = new StreamWriter(streamIn);
                    writer.Write(originalXml);
                    writer.Flush();

                    var formProtection = new FormsIntegrityUtility(signature, "PLACEHOLDER");
                    formProtection.SignForms(streamIn, streamOut);

                    streamOut.Seek(0, SeekOrigin.Begin);
                    using (var reader = new StreamReader(streamOut))
                    {
                        var xmlOut = reader.ReadToEnd();
                        Assert.AreEqual(expectedXml, xmlOut);
                    }
                }
            }
        }

        private class StringLengthSignature : IGenerateSignature
        {
            public static readonly IGenerateSignature Instance = new StringLengthSignature();

            private StringLengthSignature()
            {
            }

            public string GenerateSignature(string value)
            {
                return value.Length.ToString();
            }
        }
    }
}
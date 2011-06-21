using System.IO;
using System.Text;
using NUnit.Framework;
using Restbucks.MediaType;
using Restbucks.MediaType.Assemblers;
using Restbucks.Quoting.Service.Configuration;

namespace Tests.Restbucks.Quoting.Service.Configuration
{
    [TestFixture]
    public class RestbucksMediaTypeFormatterRequestHandlingTests
    {
        [Test]
        public void ShouldReturnNullWhenStreamIsEmpty()
        {
            const string xml = @"";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                stream.Seek(0, SeekOrigin.Begin);
                var mediaTypeProcessor = RestbucksMediaTypeFormatter.Instance;

                Assert.IsNull(mediaTypeProcessor.ReadFromStream(typeof (Shop), stream, null));
            }
        }

        [Test]
        public void ShouldReturnNullWhenStreamIsNull()
        {
            var mediaTypeProcessor = RestbucksMediaTypeFormatter.Instance;
            Assert.IsNull(mediaTypeProcessor.ReadFromStream(typeof (Shop), null, null));
        }

        [Test]
        public void ShouldReturnNullWhenStreamContainsInvalidXml()
        {
            const string xml = @"<invalid-xml><data/></invalid-xml>";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                stream.Seek(0, SeekOrigin.Begin);
                var mediaTypeProcessor = RestbucksMediaTypeFormatter.Instance;

                Assert.IsNull(mediaTypeProcessor.ReadFromStream(typeof (Shop), stream, null));
            }
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (InvalidFormatException), ExpectedMessage = "Incorrectly formatted entity body. Request must be formatted according to application/vnd.restbucks+xml.")]
        public void ShouldThrowExceptionWhenStreamContainsNonXmlData()
        {
            const string xml = @"non-xml-data";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                stream.Seek(0, SeekOrigin.Begin);
                var mediaTypeProcessor = RestbucksMediaTypeFormatter.Instance;

                mediaTypeProcessor.ReadFromStream(typeof (Shop), stream, null);
            }
        }
    }
}
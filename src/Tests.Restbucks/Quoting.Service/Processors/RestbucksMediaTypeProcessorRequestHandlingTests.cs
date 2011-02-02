using System.IO;
using System.Net.Http;
using System.ServiceModel.Description;
using System.Text;
using Microsoft.ServiceModel.Http;
using NUnit.Framework;
using Restbucks.MediaType.Assemblers;
using Restbucks.Quoting.Service.Processors;

namespace Tests.Restbucks.Quoting.Service.Processors
{
    [TestFixture]
    public class RestbucksMediaTypeProcessorRequestHandlingTests
    {
        [Test]
        public void ShouldReturnNullWhenStreamIsEmpty()
        {
            const string xml = @"";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                stream.Seek(0, SeekOrigin.Begin);
                var mediaTypeProcessor = CreateRestbucksMediaTypeProcessor();

                Assert.IsNull(mediaTypeProcessor.ReadFromStream(stream, new HttpRequestMessage()));
            }
        }

        [Test]
        public void ShouldReturnNullWhenStreamIsNull()
        {
            var mediaTypeProcessor = CreateRestbucksMediaTypeProcessor();
            Assert.IsNull(mediaTypeProcessor.ReadFromStream(null, new HttpRequestMessage()));
        }

        [Test]
        public void ShouldReturnNullWhenStreamContainsInvalidXml()
        {
            const string xml = @"<invalid-xml><data/></invalid-xml>";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                stream.Seek(0, SeekOrigin.Begin);
                var mediaTypeProcessor = CreateRestbucksMediaTypeProcessor();

                Assert.IsNull(mediaTypeProcessor.ReadFromStream(stream, new HttpRequestMessage()));
            }
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (InvalidFormatException), ExpectedMessage = "Incorrectly formatted entity body. Request must be formatted according to application/restbucks+xml.")]
        public void ShouldThrowExceptionWhenStreamContainsNonXmlData()
        {
            const string xml = @"non-xml-data";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                stream.Seek(0, SeekOrigin.Begin);
                var mediaTypeProcessor = CreateRestbucksMediaTypeProcessor();

                mediaTypeProcessor.ReadFromStream(stream, new HttpRequestMessage());
            }
        }

        private static RestbucksMediaTypeProcessor CreateRestbucksMediaTypeProcessor()
        {
            return new RestbucksMediaTypeProcessor(new HttpOperationDescription(), MediaTypeProcessorMode.Response);
        }
    }
}
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using NUnit.Framework;
using Restbucks.MediaType;
using Restbucks.Quoting.Service.Processors;
using Rhino.Mocks;

namespace Tests.Restbucks.Quoting.Service.Processors
{
    [TestFixture]
    public class FormsIntegrityResponseProcessorTests
    {
        [Test]
        public void UsesSuppliedFormsSigner()
        {
            ISignForms formsSigner = new DummyFormsSigner("output");

            var processor = new FormsIntegrityResponseProcessor(formsSigner);
            var response = new HttpResponseMessage {Content = new StringContent("input")};

            processor.Initialize();
            processor.Execute(new object[] {response});

            Assert.AreEqual("output", response.Content.ReadAsString());
        }

        [Test]
        public void DoesNotUseSuppliedFormsSignerIfEntityBodyIsNull()
        {
            var mockFormsSigner = MockRepository.GenerateMock<ISignForms>();

            var processor = new FormsIntegrityResponseProcessor(mockFormsSigner);
            var response = new HttpResponseMessage();

            processor.Initialize();
            processor.Execute(new object[] {response});

            mockFormsSigner.AssertWasNotCalled(fs => fs.SignForms(null, null), fs => fs.IgnoreArguments());
        }

        [Test]
        public void ShouldRetainExistingContentResponseHeaders()
        {
            var dateTime = new DateTimeOffset(new DateTime(2011, 1, 10));
            
            var processor = new FormsIntegrityResponseProcessor(new DummyFormsSigner(string.Empty));

            var response = new HttpResponseMessage { Content = new StringContent("input") };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(RestbucksMediaType.Value);          
            response.Content.Headers.Expires = dateTime;

            processor.Initialize();
            processor.Execute(new object[] { response });

            Assert.AreEqual(RestbucksMediaType.Value, response.Content.Headers.ContentType.MediaType);
            Assert.AreEqual(dateTime, response.Content.Headers.Expires.Value);
        }

        private class DummyFormsSigner : ISignForms
        {
            private readonly string output;

            public DummyFormsSigner(string output)
            {
                this.output = output;
            }

            public void SignForms(Stream streamIn, Stream streamOut)
            {
                using (var writer = new StreamWriter(streamOut))
                {
                    writer.Write(output);
                    writer.Flush();
                }
            }
        }
    }
}
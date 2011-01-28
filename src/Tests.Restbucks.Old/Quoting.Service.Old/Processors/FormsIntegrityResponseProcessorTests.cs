using System.IO;
using Microsoft.Http;
using NUnit.Framework;
using Restbucks.Quoting.Service.Old.Processors;
using Rhino.Mocks;

namespace Tests.Restbucks.Old.Quoting.Service.Old.Processors
{
    [TestFixture]
    public class FormsIntegrityResponseProcessorTests
    {
        [Test]
        public void UsesSuppliedFormsSigner()
        {
            ISignForms formsSigner = new StubFormsSigner("output");

            var processor = new FormsIntegrityResponseProcessor(formsSigner);
            var response = new HttpResponseMessage {Content = HttpContent.Create("input")};

            processor.Initialize();
            processor.Execute(new object[] {response});

            Assert.AreEqual("output", response.Content.ReadAsString());
        }

        [Test]
        public void DoesNotUseSuppliedFormsSignerIfEntityBodyIsNull()
        {
            var mocks = new MockRepository();
            var formsSigner = mocks.StrictMock<ISignForms>();

            using (mocks.Record())
            {
            }
            mocks.Playback();

            var processor = new FormsIntegrityResponseProcessor(formsSigner);
            var response = new HttpResponseMessage();

            processor.Initialize();
            processor.Execute(new object[] {response});

            mocks.VerifyAll();
        }

        private class StubFormsSigner : ISignForms
        {
            private readonly string output;

            public StubFormsSigner(string output)
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
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using NUnit.Framework;
using Restbucks.NewClient;
using Rhino.Mocks;
using Tests.Restbucks.Client.Helpers;

namespace Tests.Restbucks.NewClient
{
    [TestFixture]
    public class SubmitFormActionTests
    {
        [Test]
        public void ShouldSubmitFormToSuppliedResourceUri()
        {
            var formInfo = CreateFormInfo();

            var mockEndpoint = new MockEndpoint(new HttpResponseMessage());
            var client = new HttpClient {Channel = mockEndpoint};

            var action = new SubmitFormAction(formInfo, client);
            action.Execute();

            Assert.AreEqual(formInfo.ResourceUri, mockEndpoint.ReceivedRequest.RequestUri);
        }

        [Test]
        public void ShouldSubmitFormWithSuppliedMethod()
        {
            var formInfo = CreateFormInfo();

            var mockEndpoint = new MockEndpoint(new HttpResponseMessage());
            var client = new HttpClient { Channel = mockEndpoint };

            var action = new SubmitFormAction(formInfo, client);
            action.Execute();

            Assert.AreEqual(formInfo.Method, mockEndpoint.ReceivedRequest.Method);
        }

        [Test]
        public void ShouldSubmitFormWithSuppliedContentType()
        {
            var formInfo = CreateFormInfo();

            var mockEndpoint = new MockEndpoint(new HttpResponseMessage());
            var client = new HttpClient { Channel = mockEndpoint };

            var action = new SubmitFormAction(formInfo, client);
            action.Execute();

            Assert.AreEqual(formInfo.ContentType, mockEndpoint.ReceivedRequest.Content.Headers.ContentType);
        }

        private static IFormInfo CreateFormInfo()
        {
            var formInfo = MockRepository.GenerateStub<IFormInfo>();
            formInfo.Expect(f => f.ResourceUri).Return(new Uri("http://restbucks.com/orders"));
            formInfo.Expect(f => f.Method).Return(HttpMethod.Post);
            formInfo.Expect(f => f.ContentType).Return(new MediaTypeHeaderValue("application/atom+xml"));
            return formInfo;
        }
    }
}
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using NUnit.Framework;
using Restbucks.MediaType;
using Restbucks.NewClient.RulesEngine;
using Rhino.Mocks;
using Tests.Restbucks.Client.Helpers;

namespace Tests.Restbucks.NewClient.RulesEngine
{
    [TestFixture]
    public class SubmitFormTests
    {
        private static readonly HttpResponseMessage PreviousResponse = new HttpResponseMessage();
        private static readonly Uri ResourceUri = new Uri("http://localhost/rfq");
        private static readonly HttpMethod HttpMethod = HttpMethod.Post;
        private static readonly MediaTypeHeaderValue ContentType = new MediaTypeHeaderValue(RestbucksMediaType.Value);

        [Test]
        public void ShouldSubmitFormWithCorrectControlData()
        {
            var formInfo = new FormInfo(ResourceUri, HttpMethod, ContentType);

            var formStrategy = MockRepository.GenerateStub<IFormStrategy>();
            formStrategy.Expect(f => f.GetFormInfo(PreviousResponse)).Return(formInfo);

            var mockEndpoint = new MockEndpoint(new HttpResponseMessage());
            var client = new HttpClient {Channel = mockEndpoint};

            var submitForm = new SubmitForm(formStrategy, client);
            submitForm.Invoke(PreviousResponse);

            Assert.AreEqual(ResourceUri, mockEndpoint.ReceivedRequest.RequestUri);
            Assert.AreEqual(HttpMethod, mockEndpoint.ReceivedRequest.Method);
            Assert.AreEqual(ContentType, mockEndpoint.ReceivedRequest.Content.Headers.ContentType);
        }
    }
}
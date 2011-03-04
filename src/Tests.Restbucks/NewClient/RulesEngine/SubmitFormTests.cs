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
        [Test]
        public void ShouldSubmitFormWithCorrectControlData()
        {
            var previousResponse = new HttpResponseMessage();

            var resourceUri = new Uri("http://localhost/rfq");
            var httpMethod = HttpMethod.Post;
            var contentType = new MediaTypeHeaderValue(RestbucksMediaType.Value);
            var formInfo = new FormInfo(resourceUri, httpMethod, contentType, null, CreateContent());

            var formInfoFactory = MockRepository.GenerateStub<IFormInfoFactory>();
            formInfoFactory.Expect(f => f.CreateFormInfo(previousResponse)).Return(formInfo);

            var mockEndpoint = new MockEndpoint(new HttpResponseMessage());
            var client = new HttpClient {Channel = mockEndpoint};

            var submitForm = new SubmitForm(formInfoFactory, client);
            submitForm.Execute(previousResponse);

            Assert.AreEqual(resourceUri, mockEndpoint.ReceivedRequest.RequestUri);
            Assert.AreEqual(httpMethod, mockEndpoint.ReceivedRequest.Method);
            Assert.AreEqual(contentType, mockEndpoint.ReceivedRequest.Content.Headers.ContentType);
        }

        private static HttpContent CreateContent()
        {
            var content = new StringContent(string.Empty);
            content.Headers.ContentType = new MediaTypeHeaderValue(RestbucksMediaType.Value);
            return content;
        }
    }
}
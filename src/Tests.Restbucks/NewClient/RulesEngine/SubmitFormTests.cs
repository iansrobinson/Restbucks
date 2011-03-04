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
        private static readonly EntityTagHeaderValue Etag = new EntityTagHeaderValue(@"""xyz""");

        [Test]
        public void ShouldSubmitFormWithCorrectControlData()
        {
            var formInfo = new FormInfo(ResourceUri, HttpMethod, ContentType, null, CreateContent());

            var formInfoFactory = MockRepository.GenerateStub<IFormInfoFactory>();
            formInfoFactory.Expect(f => f.CreateFormInfo(PreviousResponse)).Return(formInfo);

            var mockEndpoint = new MockEndpoint(new HttpResponseMessage());
            var client = new HttpClient {Channel = mockEndpoint};

            var submitForm = new SubmitForm(formInfoFactory, client);
            submitForm.Execute(PreviousResponse);

            Assert.AreEqual(ResourceUri, mockEndpoint.ReceivedRequest.RequestUri);
            Assert.AreEqual(HttpMethod, mockEndpoint.ReceivedRequest.Method);
            Assert.AreEqual(ContentType, mockEndpoint.ReceivedRequest.Content.Headers.ContentType);
        }

        [Test]
        public void ShouldDoConditionalSubmissionIfEtagHeaderIsPresentInFormInfo()
        {
            var formInfo = new FormInfo(ResourceUri, HttpMethod, ContentType, Etag, CreateContent());

            var formInfoFactory = MockRepository.GenerateStub<IFormInfoFactory>();
            formInfoFactory.Expect(f => f.CreateFormInfo(PreviousResponse)).Return(formInfo);

            var mockEndpoint = new MockEndpoint(new HttpResponseMessage());
            var client = new HttpClient { Channel = mockEndpoint };

            var submitForm = new SubmitForm(formInfoFactory, client);
            submitForm.Execute(PreviousResponse);

            Assert.IsTrue(mockEndpoint.ReceivedRequest.Headers.IfMatch.Contains(Etag));
        }

        [Test]
        public void ShouldNotDoConditionalSubmissionIfEtagHeaderIsNotPresentInFormInfo()
        {
            var formInfo = new FormInfo(ResourceUri, HttpMethod, ContentType, null, CreateContent());

            var formInfoFactory = MockRepository.GenerateStub<IFormInfoFactory>();
            formInfoFactory.Expect(f => f.CreateFormInfo(PreviousResponse)).Return(formInfo);

            var mockEndpoint = new MockEndpoint(new HttpResponseMessage());
            var client = new HttpClient { Channel = mockEndpoint };

            var submitForm = new SubmitForm(formInfoFactory, client);
            submitForm.Execute(PreviousResponse);

            Assert.IsFalse(mockEndpoint.ReceivedRequest.Headers.IfMatch.Contains(Etag));
        }

        private static HttpContent CreateContent()
        {
            var content = new StringContent(string.Empty);
            content.Headers.ContentType = new MediaTypeHeaderValue(RestbucksMediaType.Value);
            return content;
        }
    }
}
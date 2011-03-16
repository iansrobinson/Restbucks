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
        private static readonly ApplicationContext Context = new ApplicationContext();
        private static readonly IFormDataStrategy DummyFormDataStrategy = CreateDummyFormDataStrategy();

        [Test]
        public void ShouldSubmitFormWithCorrectControlData()
        {
            var formInfo = new FormInfo(ResourceUri, HttpMethod, ContentType, DummyFormDataStrategy);

            var dummyFormStrategy = MockRepository.GenerateStub<IFormStrategy>();
            dummyFormStrategy.Expect(f => f.GetFormInfo(PreviousResponse)).Return(formInfo);

            var mockEndpoint = new MockEndpoint(new HttpResponseMessage());
            var client = new HttpClient {Channel = mockEndpoint};

            var submitForm = new SubmitForm(dummyFormStrategy);
            submitForm.Execute(PreviousResponse, Context, client);

            Assert.AreEqual(ResourceUri, mockEndpoint.ReceivedRequest.RequestUri);
            Assert.AreEqual(HttpMethod, mockEndpoint.ReceivedRequest.Method);
            Assert.AreEqual(ContentType, mockEndpoint.ReceivedRequest.Content.Headers.ContentType);
        }

        [Test]
        public void ShouldUseFormDataStrategyToCreateFormData()
        {
            var mockFormDataStrategy = MockRepository.GenerateMock<IFormDataStrategy>();
            mockFormDataStrategy.Expect(s => s.CreateFormData(PreviousResponse, Context)).Return(new StringContent(string.Empty));

            var formInfo = new FormInfo(ResourceUri, HttpMethod, ContentType, mockFormDataStrategy);

            var dummyFormStrategy = MockRepository.GenerateStub<IFormStrategy>();
            dummyFormStrategy.Expect(f => f.GetFormInfo(PreviousResponse)).Return(formInfo);

            var dummyEndpoint = new MockEndpoint(new HttpResponseMessage());
            var client = new HttpClient {Channel = dummyEndpoint};

            var submitForm = new SubmitForm(dummyFormStrategy);
            submitForm.Execute(PreviousResponse, Context, client);

            mockFormDataStrategy.VerifyAllExpectations();
        }

        private static IFormDataStrategy CreateDummyFormDataStrategy()
        {
            var dummyFormDataStrategy = MockRepository.GenerateStub<IFormDataStrategy>();
            dummyFormDataStrategy.Stub(s => s.CreateFormData(PreviousResponse, Context)).Return(new StringContent(string.Empty));
            return dummyFormDataStrategy;
        }
    }
}
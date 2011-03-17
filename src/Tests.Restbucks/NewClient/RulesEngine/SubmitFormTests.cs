using System;
using System.Net.Http;
using System.Net.Http.Headers;
using NUnit.Framework;
using Restbucks.MediaType;
using Restbucks.NewClient.RulesEngine;
using Rhino.Mocks;
using Tests.Restbucks.Client.Helpers;
using Tests.Restbucks.NewClient.Util;

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
        private static readonly FormInfo DummyFormInfo = new FormInfo(ResourceUri, HttpMethod, ContentType);

        [Test]
        public void ShouldSubmitFormWithCorrectControlData()
        {
            var dummyFormStrategy = MockRepository.GenerateStub<IFormStrategy>();
            dummyFormStrategy.Expect(f => f.GetFormInfo(PreviousResponse)).Return(DummyFormInfo);
            dummyFormStrategy.Expect(f => f.GetFormDataStrategy(PreviousResponse)).Return(DummyFormDataStrategy);

            var mockEndpoint = new MockEndpoint(new HttpResponseMessage());
            var client = new HttpClient {Channel = mockEndpoint};

            var submitForm = new SubmitForm(dummyFormStrategy);
            submitForm.Execute(PreviousResponse, Context, new ClientCapabilities(client));

            Assert.AreEqual(ResourceUri, mockEndpoint.ReceivedRequest.RequestUri);
            Assert.AreEqual(HttpMethod, mockEndpoint.ReceivedRequest.Method);
            Assert.AreEqual(ContentType, mockEndpoint.ReceivedRequest.Content.Headers.ContentType);
        }

        [Test]
        public void ShouldUseFormDataStrategyToCreateFormData()
        {
            var mockFormDataStrategy = MockRepository.GenerateMock<IFormDataStrategy>();
            mockFormDataStrategy.Expect(s => s.CreateFormData(PreviousResponse, Context)).Return(new StringContent(string.Empty));

            var dummyFormStrategy = MockRepository.GenerateStub<IFormStrategy>();
            dummyFormStrategy.Expect(f => f.GetFormInfo(PreviousResponse)).Return(DummyFormInfo);
            dummyFormStrategy.Expect(f => f.GetFormDataStrategy(PreviousResponse)).Return(mockFormDataStrategy);

            var dummyEndpoint = new MockEndpoint(new HttpResponseMessage());
            var client = new HttpClient {Channel = dummyEndpoint};

            var submitForm = new SubmitForm(dummyFormStrategy);
            submitForm.Execute(PreviousResponse, Context, new ClientCapabilities(client));

            mockFormDataStrategy.VerifyAllExpectations();
        }

        private static IFormDataStrategy CreateDummyFormDataStrategy()
        {
            var dummyFormDataStrategy = MockRepository.GenerateStub<IFormDataStrategy>();
            dummyFormDataStrategy.Stub(s => s.CreateFormData(PreviousResponse, Context)).Return(
                DummyResponse.CreateResponse().Content);
            return dummyFormDataStrategy;
        }

        private class ClientCapabilities : IClientCapabilities
        {
            private readonly HttpClient client;

            public ClientCapabilities(HttpClient client)
            {
                this.client = client;
            }

            public HttpClient HttpClient
            {
                get { return client; }
            }
        }
    }
}
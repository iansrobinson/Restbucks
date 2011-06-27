using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.ApplicationServer.Http;
using NUnit.Framework;
using Restbucks.RestToolkit.RulesEngine;
using Rhino.Mocks;
using Tests.RestInPractice.RestToolkit.RulesEngine.Util;

namespace Tests.RestInPractice.RestToolkit.RulesEngine
{
    [TestFixture]
    public class SubmitFormTests
    {
        private static readonly HttpResponseMessage PreviousResponse = new HttpResponseMessage();
        private static readonly Uri ResourceUri = new Uri("http://localhost/rfq");
        private static readonly HttpMethod HttpMethod = HttpMethod.Post;
        private static readonly ApplicationStateVariables StateVariables = new ApplicationStateVariables();
        private static readonly IFormDataStrategy DummyFormDataStrategy = CreateDummyFormDataStrategy();
        private static readonly FormInfo DummyFormInfo = new FormInfo(ResourceUri, HttpMethod, ExampleMediaType.ContentType);

        [Test]
        public void ShouldSubmitFormWithCorrectControlData()
        {
            var dummyFormStrategy = MockRepository.GenerateStub<IForm>();
            dummyFormStrategy.Expect(f => f.GetFormInfo(PreviousResponse)).Return(DummyFormInfo);
            dummyFormStrategy.Expect(f => f.GetFormDataStrategy(PreviousResponse)).Return(DummyFormDataStrategy);

            var mockEndpoint = new MockEndpoint(new HttpResponseMessage());
            var client = new HttpClient {Channel = mockEndpoint};

            var submitForm = new SubmitForm(dummyFormStrategy);
            submitForm.Execute(PreviousResponse, StateVariables, new ClientCapabilities(client));

            Assert.AreEqual(ResourceUri, mockEndpoint.ReceivedRequest.RequestUri);
            Assert.AreEqual(HttpMethod, mockEndpoint.ReceivedRequest.Method);
            Assert.AreEqual(ExampleMediaType.ContentType, mockEndpoint.ReceivedRequest.Content.Headers.ContentType);
        }

        [Test]
        public void ShouldUseFormDataStrategyToCreateFormData()
        {
            var dummyEndpoint = new MockEndpoint(new HttpResponseMessage());
            var client = new HttpClient {Channel = dummyEndpoint};
            var clientCapabilities = new ClientCapabilities(client);

            var mockFormDataStrategy = MockRepository.GenerateMock<IFormDataStrategy>();
            mockFormDataStrategy.Expect(s => s.CreateFormData(PreviousResponse, StateVariables, clientCapabilities)).Return(new StringContent(string.Empty));

            var dummyFormStrategy = MockRepository.GenerateStub<IForm>();
            dummyFormStrategy.Expect(f => f.GetFormInfo(PreviousResponse)).Return(DummyFormInfo);
            dummyFormStrategy.Expect(f => f.GetFormDataStrategy(PreviousResponse)).Return(mockFormDataStrategy);

            var submitForm = new SubmitForm(dummyFormStrategy);
            submitForm.Execute(PreviousResponse, StateVariables, clientCapabilities);

            mockFormDataStrategy.VerifyAllExpectations();
        }

        private static IFormDataStrategy CreateDummyFormDataStrategy()
        {
            var dummyFormDataStrategy = MockRepository.GenerateStub<IFormDataStrategy>();
            dummyFormDataStrategy.Stub(s => s.CreateFormData(null, null, null)).IgnoreArguments()
                .Return(DummyResponse.CreateResponse().Content);
            return dummyFormDataStrategy;
        }

        private class ClientCapabilities : IClientCapabilities
        {
            private readonly HttpClient client;

            public ClientCapabilities(HttpClient client)
            {
                this.client = client;
            }

            public HttpClient GetHttpClient()
            {
                return client;
            }

            public MediaTypeFormatter GetMediaTypeFormatter(MediaTypeHeaderValue contentType)
            {
                return ExampleMediaType.Instance;
            }
        }
    }
}
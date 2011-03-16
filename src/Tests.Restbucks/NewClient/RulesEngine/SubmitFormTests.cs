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
            var context = new ApplicationContext();
            
            var formDataStrategy = MockRepository.GenerateStub<IFormDataStrategy>();
            formDataStrategy.Stub(s => s.CreateFormData(PreviousResponse, context)).Return(new StringContent(string.Empty));

            var formInfo = new FormInfo(ResourceUri, HttpMethod, ContentType, formDataStrategy);

            var formStrategy = MockRepository.GenerateStub<IFormStrategy>();
            formStrategy.Expect(f => f.GetFormInfo(PreviousResponse)).Return(formInfo);

            var mockEndpoint = new MockEndpoint(new HttpResponseMessage());
            var client = new HttpClient {Channel = mockEndpoint};

            var submitForm = new SubmitForm(formStrategy);
            submitForm.Execute(PreviousResponse, context, client);

            Assert.AreEqual(ResourceUri, mockEndpoint.ReceivedRequest.RequestUri);
            Assert.AreEqual(HttpMethod, mockEndpoint.ReceivedRequest.Method);
            Assert.AreEqual(ContentType, mockEndpoint.ReceivedRequest.Content.Headers.ContentType);
        }

        [Test]
        public void ShouldUseFormDataStrategyToCreateFormData()
        {
            var context = new ApplicationContext();
            
            var formDataStrategy = MockRepository.GenerateMock<IFormDataStrategy>();
            formDataStrategy.Expect(s => s.CreateFormData(PreviousResponse, context)).Return(new StringContent(string.Empty));

            var formInfo = new FormInfo(ResourceUri, HttpMethod, ContentType, formDataStrategy);

            var formStrategy = MockRepository.GenerateStub<IFormStrategy>();
            formStrategy.Expect(f => f.GetFormInfo(PreviousResponse)).Return(formInfo);

            var mockEndpoint = new MockEndpoint(new HttpResponseMessage());
            var client = new HttpClient {Channel = mockEndpoint};

            var submitForm = new SubmitForm(formStrategy);
            submitForm.Execute(PreviousResponse, context, client);

            formDataStrategy.VerifyAllExpectations();
        }
    }
}
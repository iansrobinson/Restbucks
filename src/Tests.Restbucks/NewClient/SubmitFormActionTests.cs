using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Net.Http;
using NUnit.Framework;
using Restbucks.Client.Formatters;
using Restbucks.MediaType;
using Restbucks.NewClient;
using Rhino.Mocks;
using Tests.Restbucks.Client.Helpers;

namespace Tests.Restbucks.NewClient
{
    [TestFixture]
    public class SubmitFormActionTests
    {
        private static readonly IFormInfo FormInfo = CreateFormInfo();
        private static readonly Shop FormData = CreateEntityBody();
        private static readonly IContentFormatter[] Formatters = GetFormatters();

        [Test]
        public void ShouldSubmitFormToSuppliedResourceUri()
        {
            var mockEndpoint = new MockEndpoint(new HttpResponseMessage());
            var client = new HttpClient {Channel = mockEndpoint};

            var action = new SubmitFormAction(FormInfo, client, FormData, Formatters);
            action.Execute();

            Assert.AreEqual(FormInfo.ResourceUri, mockEndpoint.ReceivedRequest.RequestUri);
        }

        [Test]
        public void ShouldSubmitFormWithSuppliedMethod()
        {
            var mockEndpoint = new MockEndpoint(new HttpResponseMessage());
            var client = new HttpClient {Channel = mockEndpoint};

            var action = new SubmitFormAction(FormInfo, client, FormData, Formatters);
            action.Execute();

            Assert.AreEqual(FormInfo.Method, mockEndpoint.ReceivedRequest.Method);
        }

        [Test]
        public void ShouldSubmitFormWithSuppliedContentType()
        {
            var mockEndpoint = new MockEndpoint(new HttpResponseMessage());
            var client = new HttpClient {Channel = mockEndpoint};

            var action = new SubmitFormAction(FormInfo, client, FormData, Formatters);
            action.Execute();

            Assert.AreEqual(FormInfo.ContentType, mockEndpoint.ReceivedRequest.Content.Headers.ContentType);
        }

        [Test]
        public void ShouldSubmitFormWithSuppliedContent()
        {
            var mockEndpoint = new MockEndpoint(new HttpResponseMessage());
            var client = new HttpClient {Channel = mockEndpoint};

            var action = new SubmitFormAction(FormInfo, client, FormData, Formatters);
            action.Execute();

            var receivedFormData = mockEndpoint.ReceivedRequest.Content.ReadAsObject<Shop>(RestbucksMediaTypeFormatter.Instance);

            Assert.AreEqual(FormData.Items.Count(), receivedFormData.Items.Count());
            Assert.AreEqual(FormData.Items.First().Description, receivedFormData.Items.First().Description);
        }

        private static IContentFormatter[] GetFormatters()
        {
            return new IContentFormatter[] {new DummyFormatter(), new RestbucksFormatter() };
        }

        private static Shop CreateEntityBody()
        {
            return new ShopBuilder(null).AddItem(new Item("coffee", new Amount("g", 250))).Build();
        }

        private static IFormInfo CreateFormInfo()
        {
            var formInfo = MockRepository.GenerateStub<IFormInfo>();
            formInfo.Expect(f => f.ResourceUri).Return(new Uri("http://restbucks.com/orders"));
            formInfo.Expect(f => f.Method).Return(HttpMethod.Post);
            formInfo.Expect(f => f.ContentType).Return(new MediaTypeHeaderValue(RestbucksMediaType.Value));
            return formInfo;
        }

        private class RestbucksFormatter : IContentFormatter
        {
            public void WriteToStream(object instance, Stream stream)
            {
                RestbucksMediaTypeFormatter.Instance.WriteToStream(instance, stream);
            }

            public object ReadFromStream(Stream stream)
            {
                return RestbucksMediaTypeFormatter.Instance.ReadFromStream(stream);
            }

            public IEnumerable<MediaTypeHeaderValue> SupportedMediaTypes
            {
                get
                {
                    return new[] {new MediaTypeHeaderValue("application/restbucks+xml")}
                        .Concat(RestbucksMediaTypeFormatter.Instance.SupportedMediaTypes);
                }
            }
        }

        private class DummyFormatter : IContentFormatter
        {
            public void WriteToStream(object instance, Stream stream)
            {
                throw new NotImplementedException();
            }

            public object ReadFromStream(Stream stream)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<MediaTypeHeaderValue> SupportedMediaTypes
            {
                get
                {
                    return new[]
                               {
                                   new MediaTypeHeaderValue("application/xml"),
                                   new MediaTypeHeaderValue("text/html")
                               };
                }
            }
        }
    }
}
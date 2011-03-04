using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Net.Http;
using NUnit.Framework;
using Restbucks.Client.Formatters;
using Restbucks.MediaType;
using Restbucks.NewClient;
using Restbucks.NewClient.RulesEngine;

namespace Tests.Restbucks.NewClient
{
    [TestFixture]
    public class RestbucksFormTests
    {
        private static readonly Uri ResourceUri = new Uri("http://localhost/rfq");
        private static readonly HttpMethod Method = HttpMethod.Put;
        private static readonly MediaTypeHeaderValue ContentType = new MediaTypeHeaderValue(RestbucksMediaType.Value);

        private static readonly Shop FormBody = new ShopBuilder(new Uri("http://localhost")).Build();
        private static readonly Shop Input = new ShopBuilder(new Uri("http://restbucks.com")).Build();

        [Test]
        public void ShouldReturnCorrectFormInfoForForm()
        {
            var content = CreateContent();

            var form = new RestbucksForm("rfq");
            var formInfo = form.GetFormInfo(new HttpResponseMessage { Content = content }, new ApplicationContext(Input), new HttpContentAdapter(RestbucksMediaTypeFormatter.Instance));

            Assert.AreEqual(ContentType,  formInfo.ContentType);
            Assert.AreEqual(null, formInfo.Etag);
            Assert.AreEqual(Method, formInfo.Method);
            Assert.AreEqual(ResourceUri, formInfo.ResourceUri);
        }

        [Test]
        public void FormDataShouldContainInputIfFormBodyIsEmpty()
        {
            var content = CreateContent();

            var form = new RestbucksForm("rfq");
            var formInfo = form.GetFormInfo(new HttpResponseMessage { Content = content }, new ApplicationContext(Input), new HttpContentAdapter(RestbucksMediaTypeFormatter.Instance));

            Assert.AreEqual(Input.BaseUri, formInfo.FormData.ReadAsObject<Shop>(RestbucksMediaTypeFormatter.Instance).BaseUri);
        }

        [Test]
        public void FormDataShouldContainFormBodyIfFormBodyIsNotEmpty()
        {
            var content = CreateContent();

            var form = new RestbucksForm("order");
            var formInfo = form.GetFormInfo(new HttpResponseMessage { Content = content }, new ApplicationContext(Input), new HttpContentAdapter(RestbucksMediaTypeFormatter.Instance));

            Assert.AreEqual(FormBody.BaseUri, formInfo.FormData.ReadAsObject<Shop>(RestbucksMediaTypeFormatter.Instance).BaseUri);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (FormNotFoundException), ExpectedMessage = "Could not find form with id 'xyz'.")]
        public void ThrowsExceptionIfFormCannotBeFound()
        {
            var content = CreateContent();
            
            var form = new RestbucksForm("xyz");
            form.GetFormInfo(new HttpResponseMessage { Content = content }, new ApplicationContext(new object()), new HttpContentAdapter(RestbucksMediaTypeFormatter.Instance));
        }

        private static HttpContent CreateContent()
        {
            var entityBody = new ShopBuilder(null)
                .AddForm(new Form("rfq", ResourceUri, Method.ToString(), ContentType.MediaType, null as Shop))
                .AddForm(new Form("order", new Uri("http://localhost/orders"), "post", "application/vnd.restbucks+xml", FormBody))
                .Build();
            var content = entityBody.ToContent(RestbucksMediaTypeFormatter.Instance);
            content.Headers.ContentType = ContentType;
            return content;
        }
    }
}
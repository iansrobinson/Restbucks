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
        private static readonly Uri ResourceUri = new Uri("request-for-quote", UriKind.Relative);
        private static readonly HttpMethod Method = HttpMethod.Put;
        private static readonly MediaTypeHeaderValue ContentType = new MediaTypeHeaderValue(RestbucksMediaType.Value);

        private static readonly Shop FormBody = new ShopBuilder(new Uri("http://localhost")).Build();

        [Test]
        public void ShouldReturnCorrectFormInfoForForm()
        {
            var form = RestbucksForm.WithId("rfq");
            var formInfo = form.GetFormInfo(CreateResponse());

            Assert.AreEqual(ContentType,  formInfo.ContentType);
            Assert.AreEqual(Method, formInfo.Method);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ControlNotFoundException), ExpectedMessage = "Could not find form with id 'xyz'.")]
        public void ThrowsExceptionIfFormCannotBeFound()
        {
            var form = RestbucksForm.WithId("xyz");
            form.GetFormInfo(CreateResponse());
        }

        [Test]
        public void ShouldFormatRelativeResourceUriAsAbsoluteUri()
        {
            var form = RestbucksForm.WithId("rfq");
            var formInfo = form.GetFormInfo(CreateResponse());

            Assert.AreEqual(ContentType, formInfo.ContentType);
            Assert.AreEqual(Method, formInfo.Method);
            Assert.AreEqual(new Uri("htp://localhost/virtual-directory/request-for-quote"), formInfo.ResourceUri);
        }

        [Test]
        public void ShouldReturnTrueIfFormExists()
        {
            var form = RestbucksForm.WithId("rfq");         
            Assert.IsTrue(form.FormExists(CreateResponse()));
        }

        [Test]
        public void ShouldReturnFalseIfFormDoesNotExist()
        {
            var form = RestbucksForm.WithId("xyz");
            Assert.IsFalse(form.FormExists(CreateResponse()));
        }

        private static HttpResponseMessage CreateResponse()
        {
            var entityBody = new ShopBuilder(new Uri("htp://localhost/virtual-directory/"))
                .AddForm(new Form("rfq", ResourceUri, Method.ToString(), ContentType.MediaType, null as Shop))
                .AddForm(new Form("order", new Uri("http://restbucks.com/orders"), "post", "application/vnd.restbucks+xml", FormBody))
                .Build();
            var content = entityBody.ToContent(RestbucksMediaTypeFormatter.Instance);
            content.Headers.ContentType = ContentType;

            return new HttpResponseMessage{Content = content};
        }
    }
}
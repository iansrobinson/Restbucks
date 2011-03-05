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
        private static readonly Shop Input = new ShopBuilder(new Uri("http://restbucks.com")).Build();

        [Test]
        public void ShouldReturnCorrectFormInfoForForm()
        {
            var form = new RestbucksForm("rfq");
            var formInfo = form.GetFormInfo(new HttpResponseMessage { Content = CreateContent() }, new HttpContentAdapter(RestbucksMediaTypeFormatter.Instance), new ApplicationContext(Input));

            Assert.AreEqual(ContentType,  formInfo.ContentType);
            Assert.AreEqual(Method, formInfo.Method);
        }

        [Test]
        public void FormDataShouldContainInputIfFormBodyIsEmpty()
        {
            var form = new RestbucksForm("rfq");
            var formInfo = form.GetFormInfo(new HttpResponseMessage { Content = CreateContent() }, new HttpContentAdapter(RestbucksMediaTypeFormatter.Instance), new ApplicationContext(Input));

            Assert.AreEqual(Input.BaseUri, formInfo.FormData.ReadAsObject<Shop>(RestbucksMediaTypeFormatter.Instance).BaseUri);
        }

        [Test]
        public void FormDataShouldContainFormBodyIfFormBodyIsNotEmpty()
        {
            var form = new RestbucksForm("order");
            var formInfo = form.GetFormInfo(new HttpResponseMessage { Content = CreateContent() }, new HttpContentAdapter(RestbucksMediaTypeFormatter.Instance), new ApplicationContext(Input));

            Assert.AreEqual(FormBody.BaseUri, formInfo.FormData.ReadAsObject<Shop>(RestbucksMediaTypeFormatter.Instance).BaseUri);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ControlNotFoundException), ExpectedMessage = "Could not find form with id 'xyz'.")]
        public void ThrowsExceptionIfFormCannotBeFound()
        {
            var form = new RestbucksForm("xyz");
            form.GetFormInfo(new HttpResponseMessage { Content = CreateContent() }, new HttpContentAdapter(RestbucksMediaTypeFormatter.Instance), new ApplicationContext(new object()));
        }

        [Test]
        public void ShouldFormatRelativeResourceUriAsAbsoluteUri()
        {
            var form = new RestbucksForm("rfq");
            var formInfo = form.GetFormInfo(new HttpResponseMessage { Content = CreateContent() }, new HttpContentAdapter(RestbucksMediaTypeFormatter.Instance), new ApplicationContext(Input));

            Assert.AreEqual(ContentType, formInfo.ContentType);
            Assert.AreEqual(Method, formInfo.Method);
            Assert.AreEqual(new Uri("htp://localhost/virtual-directory/request-for-quote"), formInfo.ResourceUri);
        }

        [Test]
        public void TryGetShouldReturnTrueAndSetFormInfoIfFormExists()
        {
            FormInfo formInfo;
            var form = new RestbucksForm("rfq");
            var result = form.TryGetFormInfo(new HttpResponseMessage { Content = CreateContent() }, new HttpContentAdapter(RestbucksMediaTypeFormatter.Instance), new ApplicationContext(Input), out formInfo);

            Assert.IsTrue(result);
            Assert.AreEqual(ContentType, formInfo.ContentType);
            Assert.AreEqual(Method, formInfo.Method);
        }

        [Test]
        public void TryGetShouldReturnFalseAndSetFormInfoToNullIfFormDoesNotExist()
        {
            FormInfo formInfo;
            var form = new RestbucksForm("xyz");
            var result = form.TryGetFormInfo(new HttpResponseMessage { Content = CreateContent() }, new HttpContentAdapter(RestbucksMediaTypeFormatter.Instance), new ApplicationContext(Input), out formInfo);

            Assert.IsFalse(result);
            Assert.IsNull(formInfo);
        }

        private static HttpContent CreateContent()
        {
            var entityBody = new ShopBuilder(new Uri("htp://localhost/virtual-directory/"))
                .AddForm(new Form("rfq", ResourceUri, Method.ToString(), ContentType.MediaType, null as Shop))
                .AddForm(new Form("order", new Uri("http://restbucks.com/orders"), "post", "application/vnd.restbucks+xml", FormBody))
                .Build();
            var content = entityBody.ToContent(RestbucksMediaTypeFormatter.Instance);
            content.Headers.ContentType = ContentType;
            return content;
        }
    }
}
using System;
using System.Linq;
using Microsoft.Http;
using NUnit.Framework;
using Restbucks.MediaType;
using Restbucks.Quoting.Service.Old.Resources;
using Tests.Restbucks.Old.Quoting.Service.Old.Resources.Helpers;

namespace Tests.Restbucks.Old.Quoting.Service.Old.Resources
{
    [TestFixture]
    public class RequestForQuoteTests
    {
        private static readonly Uri BaseAddress = new Uri("http://localhost:8080/virtual-directory/");

        [Test]
        public void EntityBodyShouldContainOneForm()
        {
            var entityBody = GetRequestForQuoteEntityBody();
            Assert.AreEqual(1, entityBody.Forms.Count());
        }

        [Test]
        public void FormShouldBeEmpty()
        {
            var entityBody = GetRequestForQuoteEntityBody();
            var form = entityBody.Forms.First();

            Assert.IsNull(form.Instance);
        }

        [Test]
        public void FormShouldIndicateFormDataMustBeFormattedAccordingToRestbucksMediaType()
        {
            var entityBody = GetRequestForQuoteEntityBody();
            var form = entityBody.Forms.First();

            Assert.AreEqual(RestbucksMediaType.Value, form.MediaType);
        }

        [Test]
        public void FormShouldIndicateFormDataMustConformToRestbucksShopSchema()
        {
            var entityBody = GetRequestForQuoteEntityBody();
            var form = entityBody.Forms.First();

            Assert.AreEqual(new Uri("http://schemas.restbucks.com/shop.xsd"), form.Schema);
        }

        [Test]
        public void FormTargetShouldBeQuotesResourceWithTrailingBackslash()
        {
            var entityBody = GetRequestForQuoteEntityBody();
            var form = entityBody.Forms.First();

            Assert.AreEqual(new Uri("/quotes/", UriKind.Relative), form.Resource);
        }

        [Test]
        public void FormShouldRequirePost()
        {
            var entityBody = GetRequestForQuoteEntityBody();
            var form = entityBody.Forms.First();

            Assert.AreEqual("post", form.Method);
        }

        [Test]
        public void EntityBodyShouldNotContainAnyLinks()
        {
            var entityBody = GetRequestForQuoteEntityBody();
            Assert.IsFalse(entityBody.HasLinks);
        }

        [Test]
        public void EntityBodyShouldNotContainAnyItems()
        {
            var entityBody = GetRequestForQuoteEntityBody();
            Assert.IsFalse(entityBody.HasItems);
        }

        [Test]
        public void ResponseShouldBePublicallyCacheableForOneDay()
        {
            var resource = new RequestForQuote(DefaultUriFactory.Instance);
            var response = new HttpResponseMessage();
            resource.Get(new HttpRequestMessage("GET", GetRequestUri()), response);

            Assert.AreEqual(new TimeSpan(24, 0, 0), response.Headers.CacheControl.MaxAge);
            Assert.IsTrue(response.Headers.CacheControl.Public);
        }

        [Test]
        public void EntityBodyShouldContainBaseUri()
        {
            var entityBody = GetRequestForQuoteEntityBody();
            Assert.AreEqual(BaseAddress, entityBody.BaseUri);
        }

        private static Shop GetRequestForQuoteEntityBody()
        {
            return new RequestForQuote(DefaultUriFactory.Instance).Get(new HttpRequestMessage("GET", GetRequestUri()), new HttpResponseMessage());
        }

        private static Uri GetRequestUri()
        {
            return DefaultUriFactory.Instance.CreateAbsoluteUri<RequestForQuote>(BaseAddress);
        }
    }
}
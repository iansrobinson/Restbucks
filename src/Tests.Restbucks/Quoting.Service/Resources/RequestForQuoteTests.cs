using System;
using System.Linq;
using System.Net.Http;
using NUnit.Framework;
using Restbucks.MediaType;
using Restbucks.Quoting.Service.Resources;
using Tests.Restbucks.Quoting.Service.Resources.Helpers;

namespace Tests.Restbucks.Quoting.Service.Resources
{
    [TestFixture]
    public class RequestForQuoteTests
    {
        private static readonly Uri BaseAddress = new Uri("http://localhost:8080/virtual-directory/");

        [Test]
        public void EntityBodyShouldContainOneForm()
        {
            var entityBody = ExecuteRequestReturnEntityBody();
            Assert.AreEqual(1, entityBody.Forms.Count());
        }

        [Test]
        public void FormShouldBeEmpty()
        {
            var entityBody = ExecuteRequestReturnEntityBody();
            var form = entityBody.Forms.First();

            Assert.IsNull(form.Instance);
        }

        [Test]
        public void FormShouldIndicateFormDataMustBeFormattedAccordingToRestbucksMediaType()
        {
            var entityBody = ExecuteRequestReturnEntityBody();
            var form = entityBody.Forms.First();

            Assert.AreEqual(RestbucksMediaType.Value, form.MediaType);
        }

        [Test]
        public void FormShouldIndicateFormDataMustConformToRestbucksShopSchema()
        {
            var entityBody = ExecuteRequestReturnEntityBody();
            var form = entityBody.Forms.First();

            Assert.AreEqual(new Uri("http://schemas.restbucks.com/shop.xsd"), form.Schema);
        }

        [Test]
        public void FormTargetShouldBeQuotesResourceWithoutTrailingBackslash()
        {
            var entityBody = ExecuteRequestReturnEntityBody();
            var form = entityBody.Forms.First();

            Assert.AreEqual(new Uri("/quotes", UriKind.Relative), form.Resource);
        }

        [Test]
        public void FormShouldRequirePost()
        {
            var entityBody = ExecuteRequestReturnEntityBody();
            var form = entityBody.Forms.First();

            Assert.AreEqual("post", form.Method);
        }

        [Test]
        public void EntityBodyShouldNotContainAnyLinks()
        {
            var entityBody = ExecuteRequestReturnEntityBody();
            Assert.IsFalse(entityBody.HasLinks);
        }

        [Test]
        public void EntityBodyShouldNotContainAnyItems()
        {
            var entityBody = ExecuteRequestReturnEntityBody();
            Assert.IsFalse(entityBody.HasItems);
        }

        [Test]
        public void ResponseShouldBePublicallyCacheableForOneDay()
        {
            var response = ExecuteRequestReturnResponse();

            Assert.AreEqual(new TimeSpan(24, 0, 0), response.Headers.CacheControl.MaxAge);
            Assert.IsTrue(response.Headers.CacheControl.Public);
        }

        [Test]
        public void EntityBodyShouldIncludeBaseUri()
        {
            var entityBody = ExecuteRequestReturnEntityBody();
            Assert.AreEqual(BaseAddress, entityBody.BaseUri);
        }

        private static Shop ExecuteRequestReturnEntityBody()
        {
            var request = new HttpRequestMessage { RequestUri = DefaultUriFactory.Instance.CreateAbsoluteUri<RequestForQuote>(BaseAddress) };
            var response = new HttpResponseMessage();
            var requestForQuote = new RequestForQuote(DefaultUriFactory.Instance);
            return requestForQuote.Get(request, response);
        }

        private static HttpResponseMessage ExecuteRequestReturnResponse()
        {
            var request = new HttpRequestMessage { RequestUri = DefaultUriFactory.Instance.CreateAbsoluteUri<RequestForQuote>(BaseAddress) };
            var response = new HttpResponseMessage();
            var requestForQuote = new RequestForQuote(DefaultUriFactory.Instance);
            requestForQuote.Get(request, response);

            return response;
        }
    }
}
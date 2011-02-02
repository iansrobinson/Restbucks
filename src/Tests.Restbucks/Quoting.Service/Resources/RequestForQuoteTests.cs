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
        public void EntityBodyShouldContainAnEmptyRequestForQuoteForm()
        {
            var entityBody = ExecuteRequestReturnEntityBody();

            Assert.AreEqual(1, entityBody.Forms.Count());

            var form = entityBody.Forms.First();

            Assert.AreEqual("http://schemas.restbucks.com/shop.xsd", form.Schema.ToString());
            Assert.AreEqual(DefaultUriFactoryCollection.Instance.For<Quotes>().CreateRelativeUri(), form.Resource.ToString());
            Assert.AreEqual("post", form.Method);
            Assert.AreEqual("application/restbucks+xml", form.MediaType);
            Assert.IsNull(form.Instance);
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
            var request = new HttpRequestMessage { RequestUri = DefaultUriFactoryCollection.Instance.For<RequestForQuote>().CreateAbsoluteUri(BaseAddress) };
            var response = new HttpResponseMessage();
            var requestForQuote = new RequestForQuote(DefaultUriFactoryCollection.Instance);
            return requestForQuote.Get(request, response);
        }

        private static HttpResponseMessage ExecuteRequestReturnResponse()
        {
            var request = new HttpRequestMessage { RequestUri = DefaultUriFactoryCollection.Instance.For<RequestForQuote>().CreateAbsoluteUri(BaseAddress) };
            var response = new HttpResponseMessage();
            var requestForQuote = new RequestForQuote(DefaultUriFactoryCollection.Instance);
            requestForQuote.Get(request, response);

            return response;
        }
    }
}
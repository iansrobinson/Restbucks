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
        [Test]
        public void EntityBodyShouldContainAnEmptyRequestForQuoteForm()
        {
            var entityBody = GetRequestForQuoteEntityBody();

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
            var resource = new RequestForQuote(DefaultUriFactoryCollection.Instance);
            var response = new HttpResponseMessage();
            resource.Get(new HttpRequestMessage(HttpMethod.Get, new Uri("http://localhost/request-for-quote")), response);

            Assert.AreEqual(new TimeSpan(24, 0, 0), response.Headers.CacheControl.MaxAge);
            Assert.IsTrue(response.Headers.CacheControl.Public);
        }

        private static Shop GetRequestForQuoteEntityBody()
        {
            return new RequestForQuote(DefaultUriFactoryCollection.Instance).Get(new HttpRequestMessage(HttpMethod.Get, new Uri("http://localhost/request-for-quote")), new HttpResponseMessage());
        }
    }
}
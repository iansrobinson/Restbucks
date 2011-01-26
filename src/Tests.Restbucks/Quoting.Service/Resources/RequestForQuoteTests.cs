using System;
using System.Linq;
using System.Net.Http;
using NUnit.Framework;
using Restbucks.MediaType;
using Restbucks.Quoting.Service.Resources;

namespace Tests.Restbucks.Quoting.Service.Resources
{
    [TestFixture]
    public class RequestForQuoteTests
    {
        [Test]
        public void EntityBodyShouldContainAnEmptyRequestForQuoteForm()
        {
            var resource = BuildRequestForQuoteResource();
            Shop entityBody = resource.Get(new HttpResponseMessage());

            Assert.AreEqual(1, entityBody.Forms.Count());

            Form form = entityBody.Forms.First();

            Assert.AreEqual("http://schemas.restbucks.com/shop.xsd", form.Schema.ToString());
            Assert.AreEqual(DefaultUriFactoryCollection.Instance.For<Quotes>().CreateRelativeUri(), form.Resource.ToString());
            Assert.AreEqual("post", form.Method);
            Assert.AreEqual("application/restbucks+xml", form.MediaType);
            Assert.IsNull(form.Instance);
        }

        [Test]
        public void EntityBodyShouldNotContainAnyLinks()
        {
            var resource = BuildRequestForQuoteResource();
            Shop entityBody = resource.Get(new HttpResponseMessage());

            Assert.IsFalse(entityBody.HasLinks);
        }

        [Test]
        public void EntityBodyShouldNotContainAnyItems()
        {
            var resource = BuildRequestForQuoteResource();
            Shop entityBody = resource.Get(new HttpResponseMessage());

            Assert.IsFalse(entityBody.HasItems);
        }

        [Test]
        public void ResponseShouldBePublicallyCacheableForOneDay()
        {
            var resource = BuildRequestForQuoteResource();
            var response = new HttpResponseMessage();
            resource.Get(response);

            Assert.AreEqual(new TimeSpan(24, 0, 0), response.Headers.CacheControl.MaxAge);
            Assert.IsTrue(response.Headers.CacheControl.Public);
        }

        public static RequestForQuote BuildRequestForQuoteResource()
        {
            return new RequestForQuote(DefaultUriFactoryCollection.Instance);
        }
    }
}
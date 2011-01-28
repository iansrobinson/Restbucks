using System;
using System.Linq;
using Microsoft.Http;
using NUnit.Framework;
using Restbucks.MediaType;
using Restbucks.Quoting.Service.Old.Resources;

namespace Tests.Restbucks.Old.Quoting.Service.Old.Resources
{
    [TestFixture]
    public class RequestForQuoteTests
    {
        [Test]
        public void EntityBodyShouldContainAnEmptyRequestForQuoteForm()
        {
            var resource = new RequestForQuote();
            Shop entityBody = resource.Get(new HttpResponseMessage());

            Assert.AreEqual(1, entityBody.Forms.Count());

            Form form = entityBody.Forms.First();

            Assert.AreEqual("http://schemas.restbucks.com/shop.xsd", form.Schema.ToString());
            Assert.AreEqual(Quotes.QuotesUriFactory.CreateRelativeUri(), form.Resource.ToString());
            Assert.AreEqual("post", form.Method);
            Assert.AreEqual("application/restbucks+xml", form.MediaType);
            Assert.IsNull(form.Instance);
        }

        [Test]
        public void EntityBodyShouldNotContainAnyLinks()
        {
            var resource = new RequestForQuote();
            Shop entityBody = resource.Get(new HttpResponseMessage());

            Assert.IsFalse(entityBody.HasLinks);
        }

        [Test]
        public void EntityBodyShouldNotContainAnyItems()
        {
            var resource = new RequestForQuote();
            Shop entityBody = resource.Get(new HttpResponseMessage());

            Assert.IsFalse(entityBody.HasItems);
        }

        [Test]
        public void ResponseShouldBePublicallyCacheableForOneDay()
        {
            var resource = new RequestForQuote();
            var response = new HttpResponseMessage();
            resource.Get(response);

            Assert.AreEqual(new TimeSpan(24, 0, 0), response.Headers.CacheControl.MaxAge);
            Assert.IsTrue(response.Headers.CacheControl.Public);
        }
    }
}
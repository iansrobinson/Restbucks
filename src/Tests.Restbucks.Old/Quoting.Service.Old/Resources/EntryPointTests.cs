using System;
using System.Linq;
using Microsoft.Http;
using NUnit.Framework;
using Restbucks.MediaType;
using Restbucks.Quoting.Service.Old;
using Restbucks.Quoting.Service.Old.Resources;

namespace Tests.Restbucks.Old.Quoting.Service.Old.Resources
{
    [TestFixture]
    public class EntryPointTests
    {
        [Test]
        public void EntityBodyShouldContainALinkToRequestForQuoteForm()
        {
            var resource = new EntryPoint();
            Shop entityBody = resource.Get(new HttpResponseMessage());

            Assert.AreEqual(1, entityBody.Links.Count());

            Link link = entityBody.Links.First();

            Assert.AreEqual(RequestForQuote.UriFactory.CreateRelativeUri(), link.Href.ToString());
            Assert.AreEqual(LinkRelations.Rfq, link.Rels.First());
            Assert.AreEqual(LinkRelations.Prefetch, link.Rels.Last());
            Assert.IsNull(link.MediaType);
        }

        [Test]
        public void EntityBodyShouldNotContainAnyForms()
        {
            var resource = new EntryPoint();
            Shop entityBody = resource.Get(new HttpResponseMessage());

            Assert.IsFalse(entityBody.HasForms);
        }

        [Test]
        public void EntityBodyShouldNotContainAnyItems()
        {
            var resource = new EntryPoint();
            Shop entityBody = resource.Get(new HttpResponseMessage());

            Assert.IsFalse(entityBody.HasItems);
        }

        [Test]
        public void ResponseShouldBePublicallyCacheableForOneDay()
        {
            var resource = new EntryPoint();
            var response = new HttpResponseMessage();
            resource.Get(response);

            Assert.AreEqual(new TimeSpan(24, 0, 0), response.Headers.CacheControl.MaxAge);
            Assert.IsTrue(response.Headers.CacheControl.Public);
        }
    }
}
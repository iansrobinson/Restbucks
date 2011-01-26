using System;
using System.Linq;
using System.Net.Http;
using NUnit.Framework;
using Restbucks.MediaType;
using Restbucks.Quoting.Service;
using Restbucks.Quoting.Service.Resources;

namespace Tests.Restbucks.Quoting.Service.Resources
{
    [TestFixture]
    public class EntryPointTests
    {
        [Test]
        public void EntityBodyShouldContainALinkToRequestForQuoteForm()
        {
            var resource = BuildEntryPointResource();
            Shop entityBody = resource.Get(new HttpResponseMessage());

            Assert.AreEqual(1, entityBody.Links.Count());

            Link link = entityBody.Links.First();

            Assert.AreEqual(DefaultUriFactoryCollection.Instance.For<RequestForQuote>().CreateRelativeUri(), link.Href.ToString());
            Assert.AreEqual(LinkRelations.Rfq, link.Rels.First());
            Assert.AreEqual(LinkRelations.Prefetch, link.Rels.Last());
            Assert.IsNull(link.MediaType);
        }

        [Test]
        public void EntityBodyShouldNotContainAnyForms()
        {
            var resource = BuildEntryPointResource();
            Shop entityBody = resource.Get(new HttpResponseMessage());

            Assert.IsFalse(entityBody.HasForms);
        }

        [Test]
        public void EntityBodyShouldNotContainAnyItems()
        {
            var resource = BuildEntryPointResource();
            Shop entityBody = resource.Get(new HttpResponseMessage());

            Assert.IsFalse(entityBody.HasItems);
        }

        [Test]
        public void ResponseShouldBePublicallyCacheableForOneDay()
        {
            var resource = BuildEntryPointResource();
            var response = new HttpResponseMessage();
            resource.Get(response);

            Assert.AreEqual(new TimeSpan(24, 0, 0), response.Headers.CacheControl.MaxAge);
            Assert.IsTrue(response.Headers.CacheControl.Public);
        }

        private static EntryPoint BuildEntryPointResource()
        {
            return new EntryPoint(DefaultUriFactoryCollection.Instance);
        }
    }
}
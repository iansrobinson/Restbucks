using System;
using System.Linq;
using System.Net.Http;
using NUnit.Framework;
using Restbucks.MediaType;
using Restbucks.Quoting.Service;
using Restbucks.Quoting.Service.Resources;
using Tests.Restbucks.Quoting.Service.Resources.Util;

namespace Tests.Restbucks.Quoting.Service.Resources
{
    [TestFixture]
    public class EntryPointTests
    {
        [Test]
        public void EntityBodyShouldContainOneLink()
        {
            var entityBody = GetEntryPointEntityBody();
            Assert.AreEqual(1, entityBody.Links.Count());
        }

        [Test]
        public void LinkHrefShouldPointToRequestForQuoteResourceWithoutTrailingBackslash()
        {
            var entityBody = GetEntryPointEntityBody();
            var link = entityBody.Links.First();

            Assert.AreEqual(new Uri("request-for-quote", UriKind.Relative), link.Href);
        }

        [Test]
        public void LinkShouldIndicateTargetIsRequestForQuoteForm()
        {
            var entityBody = GetEntryPointEntityBody();
            var link = entityBody.Links.First();

            Assert.AreEqual(LinkRelations.Rfq, link.Rels.First());
        }

        [Test]
        public void LinkShouldIndicateItCanBePrefetched()
        {
            var entityBody = GetEntryPointEntityBody();
            var link = entityBody.Links.First();

            Assert.AreEqual(LinkRelations.Prefetch, link.Rels.Last());
        }

        [Test]
        public void LinkShouldIndicateResponseWillLikelyBeFormattedAccordingToRestbucksMediaType()
        {
            var entityBody = GetEntryPointEntityBody();
            var link = entityBody.Links.First();

            Assert.AreEqual(RestbucksMediaType.Value, link.MediaType);
        }

        [Test]
        public void EntityBodyShouldNotContainAnyForms()
        {
            var entityBody = GetEntryPointEntityBody();
            Assert.IsFalse(entityBody.HasForms);
        }

        [Test]
        public void EntityBodyShouldNotContainAnyItems()
        {
            var entityBody = GetEntryPointEntityBody();
            Assert.IsFalse(entityBody.HasItems);
        }

        [Test]
        public void ResponseShouldBePublicallyCacheableForOneDay()
        {
            var resource = new EntryPoint(DefaultUriFactory.Instance);
            var response = new HttpResponseMessage();
            resource.Get(new HttpRequestMessage(HttpMethod.Get, new Uri("http://localhost/shop")), response);

            Assert.AreEqual(new TimeSpan(24, 0, 0), response.Headers.CacheControl.MaxAge);
            Assert.IsTrue(response.Headers.CacheControl.Public);
        }

        [Test]
        public void BaseUriShouldIncludeVirtualDirectoryIfPresent()
        {
            var resource = new EntryPoint(DefaultUriFactory.Instance);
            var entityBody = resource.Get(new HttpRequestMessage(HttpMethod.Get, new Uri("http://localhost/virtual-directory/shop")), new HttpResponseMessage());

            Assert.AreEqual(new Uri("http://localhost/virtual-directory/"), entityBody.BaseUri);
        }

        private static Shop GetEntryPointEntityBody()
        {
            return new EntryPoint(DefaultUriFactory.Instance).Get(new HttpRequestMessage(HttpMethod.Get, new Uri("http://localhost/shop")), new HttpResponseMessage());
        }
    }
}
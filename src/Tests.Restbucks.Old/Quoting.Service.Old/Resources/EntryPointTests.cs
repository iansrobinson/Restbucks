﻿using System;
using System.Linq;
using Microsoft.Http;
using NUnit.Framework;
using Restbucks.MediaType;
using Restbucks.Quoting.Service.Old;
using Restbucks.Quoting.Service.Old.Resources;
using Tests.Restbucks.Old.Quoting.Service.Old.Resources.Helpers;

namespace Tests.Restbucks.Old.Quoting.Service.Old.Resources
{
    [TestFixture]
    public class EntryPointTests
    {
        private static readonly Uri BaseAddress = new Uri("http://localhost:8080/virtual-directory/");

        [Test]
        public void EntityBodyShouldContainOneLink()
        {
            var entityBody = GetEntryPointEntityBody();
            Assert.AreEqual(1, entityBody.Links.Count());
        }

        [Test]
        public void LinkHrefShouldPointToRequestForQuoteResourceWithTrailingBackslash()
        {
            var entityBody = GetEntryPointEntityBody();
            var link = entityBody.Links.First();

            Assert.AreEqual(new Uri("request-for-quote/", UriKind.Relative), link.Href);
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
        public void EntityBodyShouldIncludeBaseUri()
        {
            var entityBody = GetEntryPointEntityBody();
            Assert.AreEqual(BaseAddress, entityBody.BaseUri);
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
            resource.Get(new HttpRequestMessage("GET", DefaultUriFactory.Instance.CreateAbsoluteUri<EntryPoint>(BaseAddress)), response);

            Assert.AreEqual(new TimeSpan(24, 0, 0), response.Headers.CacheControl.MaxAge);
            Assert.IsTrue(response.Headers.CacheControl.Public);
        }

        private static Shop GetEntryPointEntityBody()
        {
            return new EntryPoint(DefaultUriFactory.Instance).Get(new HttpRequestMessage("GET", DefaultUriFactory.Instance.CreateAbsoluteUri<EntryPoint>(BaseAddress)), new HttpResponseMessage());
        }
    }
}
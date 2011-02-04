using System;
using NUnit.Framework;
using Restbucks.MediaType;
using Tests.Restbucks.MediaType.Helpers;

namespace Tests.Restbucks.MediaType
{
    [TestFixture]
    public class LinkTests
    {
        [Test]
        public void PrefetchInvokesClientWithLinkHref()
        {
            Uri uriParameter = null;
            Func<Uri, Shop> client = uri =>
                                         {
                                             uriParameter = uri;
                                             return null;
                                         };

            var href = new Uri("http://localhost/shop/");
            var link = new Link(href, RestbucksMediaType.Value);

            link.Prefetch(client);

            Assert.AreEqual(href, uriParameter);
        }

        [Test]
        public void IfLinkHasBeenPrefetchedClickReturnsPrefetchedResource()
        {
            var shop = new ShopBuilder().Build();
            
            var href = new Uri("http://localhost/shop/");
            var link = new Link(href, RestbucksMediaType.Value);

            link.Prefetch(uri => shop);
            
            Assert.AreEqual(shop, link.Click(uri => null));
        }

        [Test]
        public void IfLinkhasNotBeenPrefetchedClieckFetchesResource()
        {
            var shop = new ShopBuilder().Build();

            var href = new Uri("http://localhost/shop/");
            var link = new Link(href, RestbucksMediaType.Value);

            Assert.AreEqual(shop, link.Click(uri => shop));
        }

        [Test]
        public void ClickInvokesClientWithLinkHref()
        {
            Uri uriParameter = null;
            Func<Uri, Shop> client = uri =>
            {
                uriParameter = uri;
                return null;
            };

            var href = new Uri("http://localhost/shop/");
            var link = new Link(href, RestbucksMediaType.Value);

            link.Click(client);

            Assert.AreEqual(href, uriParameter);
        }
    }
}
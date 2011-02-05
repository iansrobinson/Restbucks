//using System;
//using NUnit.Framework;
//using Restbucks.MediaType;
//using Tests.Restbucks.MediaType.Helpers;
//
//namespace Tests.Restbucks.MediaType
//{
//    [TestFixture]
//    public class LinkTests
//    {
//        [Test]
//        public void PrefetchInvokesClientWithLinkHref()
//        {
//            Uri uriParameter = null;
//            Func<Uri, Shop> client = uri =>
//                                         {
//                                             uriParameter = uri;
//                                             return null;
//                                         };
//
//            var href = new Uri("http://localhost/shop/");
//            var link = new Link(href, RestbucksMediaType.Value);
//
//            link.Prefetch(client);
//
//            Assert.AreEqual(href, uriParameter);
//        }
//
//        [Test]
//        public void IfLinkHasBeenPrefetchedClickReturnsPrefetchedResource()
//        {
//            var shop = new ShopBuilder().Build();
//            
//            var href = new Uri("http://localhost/shop/");
//            var link = new Link(href, RestbucksMediaType.Value);
//
//            link.Prefetch(uri => shop);
//            
//            Assert.AreEqual(shop, link.Click(uri => null));
//        }
//
//        [Test]
//        public void IfLinkhasNotBeenPrefetchedClickFetchesResource()
//        {
//            var shop = new ShopBuilder().Build();
//
//            var href = new Uri("http://localhost/shop/");
//            var link = new Link(href, RestbucksMediaType.Value);
//
//            Assert.AreEqual(shop, link.Click(uri => shop));
//        }
//
//        [Test]
//        public void ClickInvokesClientWithLinkHref()
//        {
//            Uri uriParameter = null;
//            Func<Uri, Shop> client = uri =>
//            {
//                uriParameter = uri;
//                return null;
//            };
//
//            var href = new Uri("http://localhost/shop/");
//            var link = new Link(href, RestbucksMediaType.Value);
//
//            link.Click(client);
//
//            Assert.AreEqual(href, uriParameter);
//        }
//
//        [Test]
//        [ExpectedException(ExpectedException = typeof(InvalidOperationException), ExpectedMessage = "Unable to determine full URI.")]
//        public void ThrowsExceptionIfTryingToClickLinkWithRelativeUriAndNoKnownBaseUri()
//        {
//            var href = new Uri("/quotes", UriKind.Relative);
//            var link = new Link(href, RestbucksMediaType.Value);
//
//            link.Click(uri => null);
//        }
//
//        [Test]
//        public void ShouldCreateNewLinkWithBaseUri()
//        {
//            var href = new Uri("/quotes", UriKind.Relative);
//            var link = new Link(href, RestbucksMediaType.Value);
//
//            var newLink = link.NewLinkWithBaseUri(new Uri("http://localhost"));
//            
//            Uri clickUri = null;
//            newLink.Click(uri =>
//                              {
//                                  clickUri = uri;
//                                  return null;
//                              });
//
//            Assert.AreEqual(new Uri("http://localhost/quotes"), clickUri);
//        }
//
//        [Test]
//        public void NewLinkShouldStillHaveARelativeHref()
//        {
//            var href = new Uri("/quotes", UriKind.Relative);
//            var link = new Link(href, RestbucksMediaType.Value);
//
//            var newLink = link.NewLinkWithBaseUri(new Uri("http://localhost"));
//
//            Assert.AreEqual(href, newLink.Href);
//        }
//
//        [Test]
//        [ExpectedException(ExpectedException = typeof(InvalidOperationException), ExpectedMessage = "Href is already backed by an absolute URI.")]
//        public void ShouldThrowExceptionIfTryingToCreateNewLinkBasedOnLinkWithAbosoluteHref()
//        {
//            var href = new Uri("http://restbucks.com/quotes", UriKind.Absolute);
//            var link = new Link(href, RestbucksMediaType.Value);
//
//            link.NewLinkWithBaseUri(new Uri("http://localhost", UriKind.Absolute));
//        }
//    }
//}
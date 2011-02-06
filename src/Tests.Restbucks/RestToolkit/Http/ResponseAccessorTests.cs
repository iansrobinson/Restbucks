using System;
using System.Collections.Generic;
using NUnit.Framework;
using Restbucks.MediaType;
using Restbucks.RestToolkit.Http;
using Tests.Restbucks.MediaType.Helpers;

namespace Tests.Restbucks.RestToolkit.Http
{
    [TestFixture]
    public class ResponseAccessorTests
    {
        private static readonly Uri RequestUri = new Uri("http://localhost/quotes");

        [Test]
        public void ShouldUseClientToGetResponse()
        {
            var firstResponse = CreateResponse();

            Func<Uri, Response<Shop>, Response<Shop>> client = (uri, prevResponse) => firstResponse;

            var accessor = ResponseAccessor<Shop>.Create(RequestUri);
            var response = accessor.GetResponse(client);

            Assert.AreEqual(firstResponse, response);
        }

        [Test]
        public void ShouldPassPreviousResponseToClient()
        {
            Response<Shop> previousResponse = null;

            var firstResponse = CreateResponse();
            var secondResponse = CreateResponse();

            Func<Uri, Response<Shop>, Response<Shop>> firstClient = (uri, prevResponse) => firstResponse;
            Func<Uri, Response<Shop>, Response<Shop>> secondClient = (uri, prevResponse) =>
            {
                previousResponse = prevResponse;
                return secondResponse;
            };

            var accessor = ResponseAccessor<Shop>.Create(RequestUri);
            accessor.GetResponse(firstClient);
            accessor.GetResponse(secondClient);

            Assert.AreEqual(firstResponse, previousResponse);
        }

        [Test]
        public void ShouldReturnPrefetchedResponseWithNextGetResponse()
        {
            var firstResponse = new Response<Shop>(200, new Dictionary<string, IEnumerable<string>>(), new ShopBuilder().Build());

            Func<Uri, Response<Shop>, Response<Shop>> firstClient = (uri, prevResponse) => firstResponse;
            Func<Uri, Response<Shop>, Response<Shop>> secondClient = (uri, prevResponse) => { throw new AssertionException("Client ought not be called a second time."); };

            var accessor = ResponseAccessor<Shop>.Create(RequestUri);

            accessor.PrefetchResponse(firstClient);
            var response = accessor.GetResponse(secondClient);

            Assert.AreEqual(firstResponse, response);
        }

        [Test]
        public void PrefetchedResponseIsDiscardedAfterItHasBeenReturnedFromGetResponse()
        {
            var firstResponse = CreateResponse();
            var secondResponse = CreateResponse();

            Func<Uri, Response<Shop>, Response<Shop>> firstClient = (uri, prevResponse) => firstResponse;
            Func<Uri, Response<Shop>, Response<Shop>> secondClient = (uri, prevResponse) => { throw new AssertionException("Client ought not be called a second time."); };
            Func<Uri, Response<Shop>, Response<Shop>> thirdClient = (uri, prevResponse) => secondResponse;

            var accessor = ResponseAccessor<Shop>.Create(RequestUri);

            accessor.PrefetchResponse(firstClient);
            accessor.GetResponse(secondClient);
            var response = accessor.GetResponse(thirdClient);

            Assert.AreEqual(secondResponse, response);
        }

        [Test]
        public void ShouldPassPreviousResponseToClientWhenPrefetchingResponse()
        {
            var firstResponse = CreateResponse();
            var secondResponse = CreateResponse();

            Response<Shop> previousResponse = null;

            Func<Uri, Response<Shop>, Response<Shop>> firstClient = (uri, prevResponse) => firstResponse;
            Func<Uri, Response<Shop>, Response<Shop>> secondClient = (uri, prevResponse) => { throw new AssertionException("Client ought not be called a second time."); };
            Func<Uri, Response<Shop>, Response<Shop>> thirdClient = (uri, prevResponse) =>
                                                                        {
                                                                            previousResponse = prevResponse;
                                                                            return secondResponse;
                                                                        };

            var accessor = ResponseAccessor<Shop>.Create(RequestUri);

            accessor.PrefetchResponse(firstClient);
            accessor.GetResponse(secondClient);
            accessor.PrefetchResponse(thirdClient);

            Assert.AreEqual(firstResponse, previousResponse);
        }

        [Test]
        public void AbsoluteUriIsDereferenceable()
        {
            var accessor = ResponseAccessor<Shop>.Create(RequestUri);
            Assert.IsTrue(accessor.IsDereferenceable);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(InvalidOperationException), ExpectedMessage = "Cannot access URI. URI must be an absolute URI. Uri: [].")]
        public void WhenUriIsNullGetResponseThrowsException()
        {
            var accessor = ResponseAccessor<Shop>.Create(null);
            accessor.GetResponse((uri, prevResponse) => null);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(InvalidOperationException), ExpectedMessage = "Cannot access URI. URI must be an absolute URI. Uri: [].")]
        public void WhenUriIsNullPrefetchResponseThrowsException()
        {
            var accessor = ResponseAccessor<Shop>.Create(null);
            accessor.PrefetchResponse((uri, prevResponse) => null);
        }

        [Test]
        public void WhenUriIsNullIsNotDereferenceable()
        {
            var accessor = ResponseAccessor<Shop>.Create(null);
            Assert.IsFalse(accessor.IsDereferenceable);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(InvalidOperationException), ExpectedMessage = "Cannot access URI. URI must be an absolute URI. Uri: [/quotes].")]
        public void WhenUriIsRelativeGetResponseThrowsException()
        {
            var accessor = ResponseAccessor<Shop>.Create(new Uri("/quotes", UriKind.Relative));
            accessor.GetResponse((uri, prevResponse) => null);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(InvalidOperationException), ExpectedMessage = "Cannot access URI. URI must be an absolute URI. Uri: [/quotes].")]
        public void WhenUriIsRelativePrefetchResponseThrowsException()
        {
            var accessor = ResponseAccessor<Shop>.Create(new Uri("/quotes", UriKind.Relative));
            accessor.PrefetchResponse((uri, prevResponse) => null);
        }

        [Test]
        public void WhenUriIsRelativeIsNotDereferenceable()
        {
            var accessor = ResponseAccessor<Shop>.Create(new Uri("/quotes", UriKind.Relative));
            Assert.IsFalse(accessor.IsDereferenceable);
        }

        private static Response<Shop> CreateResponse()
        {
            return new Response<Shop>(200, new Dictionary<string, IEnumerable<string>>(), new ShopBuilder().Build());
        }
    }
}

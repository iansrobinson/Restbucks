using System;
using System.Collections.Generic;
using NUnit.Framework;
using Restbucks.MediaType;
using Restbucks.RestToolkit.Http;
using Tests.Restbucks.MediaType.Helpers;

namespace Tests.Restbucks.RestToolkit.Http
{
    [TestFixture]
    public class ResponseLifecycleControllerTests
    {
        private static readonly Uri RequestUri = new Uri("http://localhost/quotes");

        [Test]
        public void ShouldUseClientToGetResponse()
        {
            var firstResponse = CreateResponse();

            Func<Uri, Response<Shop>, Response<Shop>> client = (uri, prevResponse) => firstResponse;

            var controller = new ResponseLifecycleController<Shop>(RequestUri);
            var response = controller.GetResponse(client);

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

            var controller = new ResponseLifecycleController<Shop>(RequestUri);

            controller.GetResponse(firstClient);
            controller.GetResponse(secondClient);

            Assert.AreEqual(firstResponse, previousResponse);
        }

        [Test]
        public void ShouldReturnPrefetchedResponseWithNextGetResponse()
        {
            var firstResponse = new Response<Shop>(200, new Dictionary<string, IEnumerable<string>>(), new ShopBuilder().Build());

            Func<Uri, Response<Shop>, Response<Shop>> firstClient = (uri, prevResponse) => firstResponse;
            Func<Uri, Response<Shop>, Response<Shop>> secondClient = (uri, prevResponse) => { throw new AssertionException("Client ought not be called a second time."); };

            var controller = new ResponseLifecycleController<Shop>(RequestUri);

            controller.PrefetchResponse(firstClient);
            var response = controller.GetResponse(secondClient);

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

            var controller = new ResponseLifecycleController<Shop>(RequestUri);

            controller.PrefetchResponse(firstClient);
            controller.GetResponse(secondClient);
            var response = controller.GetResponse(thirdClient);

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

            var controller = new ResponseLifecycleController<Shop>(RequestUri);

            controller.PrefetchResponse(firstClient);
            controller.GetResponse(secondClient);
            controller.PrefetchResponse(thirdClient);

            Assert.AreEqual(firstResponse, previousResponse);
        }

        private static Response<Shop> CreateResponse()
        {
            return new Response<Shop>(200, new Dictionary<string, IEnumerable<string>>(), new ShopBuilder().Build());
        }
    }
}
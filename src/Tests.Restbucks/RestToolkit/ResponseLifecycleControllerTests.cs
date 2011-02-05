using System;
using System.Collections.Generic;
using NUnit.Framework;
using Restbucks.MediaType;
using Restbucks.RestToolkit;
using Tests.Restbucks.MediaType.Helpers;

namespace Tests.Restbucks.RestToolkit
{
    [TestFixture]
    public class ResponseLifecycleControllerTests
    {
        [Test]
        public void ShouldUseAClientToGetResponse()
        {
            var requestUri = new Uri("http://localhost/quotes");
            var entityBody = new ShopBuilder().Build();
            Func<Uri, Response<Shop>, Response<Shop>> client = (uri, prevResponse) => new Response<Shop>(200, new Dictionary<string, IEnumerable<string>>(), entityBody);

            var controller = new ResponseLifecycleController<Shop>(requestUri);
            var response = controller.GetResponse(client);

            Assert.AreEqual(entityBody, response.EntityBody);
        }

        [Test]
        public void ShouldPassPreviousResponseToClient()
        {
            var requestUri = new Uri("http://localhost/quotes");
            var entityBody = new ShopBuilder().Build();
            Response<Shop> previousResponse = null;

            Func<Uri, Response<Shop>, Response<Shop>> firstClient = (uri, prevResponse) => new Response<Shop>(200, new Dictionary<string, IEnumerable<string>>(), entityBody);
            Func<Uri, Response<Shop>, Response<Shop>> secondClient = (uri, prevResponse) =>
                                                                         {
                                                                             previousResponse = prevResponse;
                                                                             return new Response<Shop>(200, new Dictionary<string, IEnumerable<string>>(), entityBody);
                                                                         };

            var controller = new ResponseLifecycleController<Shop>(requestUri);
            var firstResponse = controller.GetResponse(firstClient);

            controller.GetResponse(secondClient);

            Assert.AreEqual(firstResponse, previousResponse);
        }

        [Test]
        public void ShouldReturnPrefetchedResponseWithNextGetResponse()
        {
            var requestUri = new Uri("http://localhost/quotes");
            var prefetchedResponse = new Response<Shop>(200, new Dictionary<string, IEnumerable<string>>(), new ShopBuilder().Build());

            Func<Uri, Response<Shop>, Response<Shop>> firstClient = (uri, prevResponse) => prefetchedResponse;
            Func<Uri, Response<Shop>, Response<Shop>> secondClient = (uri, prevResponse) =>
            {
                throw new AssertionException("Client ought not be called a second time.");
            };

            var controller = new ResponseLifecycleController<Shop>(requestUri);
            controller.PrefetchResponse(firstClient);

            var response = controller.GetResponse(secondClient);

            Assert.AreEqual(prefetchedResponse, response);
        }

    }
}
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Net.Http;
using NUnit.Framework;
using Restbucks.MediaType;
using Restbucks.NewClient;
using Restbucks.NewClient.RulesEngine;

namespace Tests.Restbucks.NewClient
{
    [TestFixture]
    public class ApplicationContextFormDataStrategyTests
    {
        [Test]
        public void ShouldRetrieveFormDataFromApplicationContextBasedOnKey()
        {
            var baseUri = new Uri("http://localhost/restbucks/");
            var entityBody = new ShopBuilder(baseUri).Build();

            var contentType = new MediaTypeHeaderValue(RestbucksMediaType.Value);
            var key = new EntityBodyKey("order-form", contentType, new Uri("http://schemas/shop"));
            var context = new ApplicationContext(new KeyValuePair<IKey, object>(key, entityBody));

            var strategy = new ApplicationContextFormDataStrategy(key, contentType);
            var content = strategy.CreateFormData(new HttpResponseMessage(), context);

            Assert.AreEqual(baseUri, content.ReadAsObject<Shop>(RestbucksFormatter.Instance).BaseUri);
        }
    }
}
﻿using System;
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
        private static readonly Shop EntityBody = new ShopBuilder(new Uri("http://localhost/restbucks/")).Build();
        private static readonly MediaTypeHeaderValue ContentType = new MediaTypeHeaderValue(RestbucksMediaType.Value);
        private static readonly EntityBodyKey Key = new EntityBodyKey("order-form", ContentType, new Uri("http://schemas/shop"));
        private static readonly ApplicationContext Context = new ApplicationContext(new KeyValuePair<IKey, object>(Key, EntityBody));

        [Test]
        public void ShouldRetrieveFormDataFromApplicationContextBasedOnKey()
        {
            var strategy = new ApplicationContextFormDataStrategy(Key, ContentType);
            var content = strategy.CreateFormData(new HttpResponseMessage(), Context);

            Assert.AreEqual(EntityBody.BaseUri, content.ReadAsObject<Shop>(RestbucksFormatter.Instance).BaseUri);
        }

        [Test]
        public void ShouldAddContentTypeHeaderToContent()
        {
            var strategy = new ApplicationContextFormDataStrategy(Key, ContentType);
            var content = strategy.CreateFormData(new HttpResponseMessage(), Context);

            Assert.AreEqual(ContentType, content.Headers.ContentType);
        }
    }
}
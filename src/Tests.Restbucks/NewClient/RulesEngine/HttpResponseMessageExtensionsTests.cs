using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Net.Http;
using NUnit.Framework;
using Restbucks.Client.Formatters;
using Restbucks.MediaType;
using Restbucks.NewClient;
using Restbucks.NewClient.RulesEngine;

namespace Tests.Restbucks.NewClient.RulesEngine
{
    [TestFixture]
    public class HttpResponseMessageExtensionsTests
    {
        [Test]
        public void ShouldReturnTrueIfLinkExists()
        {
            var response = CreateResponse();
            Assert.IsTrue(response.LinkExists(RestbucksLink.WithRel("rfq")));
        }

        [Test]
        public void ShouldReturnFalseIfLinkDoesNotExist()
        {
            var response = CreateResponse();
            Assert.IsFalse(response.LinkExists(RestbucksLink.WithRel("xyz")));
        }

        private static HttpResponseMessage CreateResponse()
        {
            var entityBody = new ShopBuilder(new Uri("http://localhost/virtual-directory/"))
                .AddLink(new Link(
                             new Uri("request-for-quote", UriKind.Relative),
                             RestbucksMediaType.Value,
                             new StringLinkRelation("rfq")))
                .Build();

            var content = entityBody.ToContent(RestbucksMediaTypeFormatter.Instance);
            content.Headers.ContentType = new MediaTypeHeaderValue(RestbucksMediaType.Value);

            return new HttpResponseMessage {Content = content};
        }
    }
}
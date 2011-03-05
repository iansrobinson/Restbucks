using System;
using System.Net.Http;
using System.Net.Http.Headers;
using NUnit.Framework;
using Restbucks.Client.Formatters;
using Restbucks.MediaType;
using Restbucks.NewClient;
using Restbucks.NewClient.RulesEngine;

namespace Tests.Restbucks.NewClient
{
    [TestFixture]
    public class RestbucksLinkTests
    {
        [Test]
        public void ShouldConvertRelativeUrisToAbsoluteUris()
        {
            var contentAdapter = new HttpContentAdapter(RestbucksMediaTypeFormatter.Instance);

            var link = new RestbucksLink(new StringLinkRelation("rfq"));
            var linkInfo = link.GetLinkInfo(new HttpResponseMessage {Content = contentAdapter.CreateContent(CreateEntityBody(), new MediaTypeHeaderValue(RestbucksMediaType.Value))}, contentAdapter);

            Assert.AreEqual(new Uri("http://localhost/virtual-directory/request-for-quote"), linkInfo.ResourceUri);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ControlNotFoundException), ExpectedMessage = "Could not find link with link relation 'xyz'.")]
        public void ThrowsExceptionIfLinkCannotBeFound()
        {
            var contentAdapter = new HttpContentAdapter(RestbucksMediaTypeFormatter.Instance);

            var link = new RestbucksLink(new StringLinkRelation("xyz"));
            var linkInfo = link.GetLinkInfo(new HttpResponseMessage {Content = contentAdapter.CreateContent(CreateEntityBody(), new MediaTypeHeaderValue(RestbucksMediaType.Value))}, contentAdapter);

            Assert.AreEqual(new Uri("http://localhost/virtual-directory/request-for-quote"), linkInfo.ResourceUri);
        }

        [Test]
        public void TryGetShouldReturnTrueAndSetLinkInfoIfLinkExists()
        {
            var contentAdapter = new HttpContentAdapter(RestbucksMediaTypeFormatter.Instance);

            var link = new RestbucksLink(new StringLinkRelation("rfq"));
            LinkInfo linkInfo;
            var result = link.TryGetLinkInfo(new HttpResponseMessage {Content = contentAdapter.CreateContent(CreateEntityBody(), new MediaTypeHeaderValue(RestbucksMediaType.Value))}, contentAdapter, out linkInfo);

            Assert.IsTrue(result);
            Assert.AreEqual(new Uri("http://localhost/virtual-directory/request-for-quote"), linkInfo.ResourceUri);
        }

        [Test]
        public void TryGetShouldReturnFalseAndSetLinkInfoToNullIfLinkDoesNotExist()
        {
            var contentAdapter = new HttpContentAdapter(RestbucksMediaTypeFormatter.Instance);

            var link = new RestbucksLink(new StringLinkRelation("xyz"));
            LinkInfo linkInfo;
            var result = link.TryGetLinkInfo(new HttpResponseMessage { Content = contentAdapter.CreateContent(CreateEntityBody(), new MediaTypeHeaderValue(RestbucksMediaType.Value)) }, contentAdapter, out linkInfo);

            Assert.IsFalse(result);
            Assert.IsNull(linkInfo);
        }

        private static Shop CreateEntityBody()
        {
            return new ShopBuilder(new Uri("http://localhost/virtual-directory/"))
                .AddLink(new Link(
                             new Uri("request-for-quote", UriKind.Relative),
                             RestbucksMediaType.Value,
                             new StringLinkRelation("rfq")))
                .Build();
        }
    }
}
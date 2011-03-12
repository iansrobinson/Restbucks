using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Net.Http;
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
            var link = RestbucksLink.WithRel("rfq");
            var linkInfo = link.GetLinkInfo(CreateResponse());

            Assert.AreEqual(new Uri("http://localhost/virtual-directory/request-for-quote"), linkInfo.ResourceUri);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ControlNotFoundException), ExpectedMessage = "Could not find link with link relation 'xyz'.")]
        public void ThrowsExceptionIfLinkCannotBeFound()
        {
            var link = RestbucksLink.WithRel("xyz");
            var linkInfo = link.GetLinkInfo(CreateResponse());

            Assert.AreEqual(new Uri("http://localhost/virtual-directory/request-for-quote"), linkInfo.ResourceUri);
        }

        [Test]
        public void ShouldReturnTrueIfLinkExists()
        {
            var link = RestbucksLink.WithRel(new Uri("http://relations/rfq"));
            var result = link.LinkExists(CreateResponse());

            Assert.IsTrue(result);
        }

        [Test]
        public void ShouldReturnFalseIfLinkDoesNotExist()
        {
            var link = RestbucksLink.WithRel(new Uri("http://relations/xyz"));
            var result = link.LinkExists(CreateResponse());

            Assert.IsFalse(result);
        }

        private static HttpResponseMessage CreateResponse()
        {
            var entityBody = new ShopBuilder(new Uri("http://localhost/virtual-directory/"))
                .AddLink(new Link(
                             new Uri("request-for-quote", UriKind.Relative),
                             RestbucksMediaType.Value,
                             new StringLinkRelation("rfq")))
                .AddLink(new Link(
                             new Uri("request-for-quote", UriKind.Relative),
                             RestbucksMediaType.Value,
                             new UriLinkRelation(new Uri("http://relations/rfq"))))
                .Build();

            var content = entityBody.ToContent(RestbucksMediaTypeFormatter.Instance);
            content.Headers.ContentType = new MediaTypeHeaderValue(RestbucksMediaType.Value);

            return new HttpResponseMessage {Content = content};
        }
    }
}
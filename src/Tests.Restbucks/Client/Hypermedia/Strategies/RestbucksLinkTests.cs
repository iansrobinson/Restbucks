using System;
using NUnit.Framework;
using Restbucks.Client.Hypermedia.Strategies;
using Restbucks.RestToolkit.RulesEngine;
using Tests.Restbucks.Client.Util;

namespace Tests.Restbucks.Client.Hypermedia.Strategies
{
    [TestFixture]
    public class RestbucksLinkTests
    {
        [Test]
        public void ShouldConvertRelativeUrisToAbsoluteUris()
        {
            var link = RestbucksLink.WithRel("http://relations.restbucks.com/rfq");
            var linkInfo = link.GetLinkInfo(DummyResponse.CreateResponse());

            Assert.AreEqual(DummyResponse.LinkAbsoluteUri, linkInfo.ResourceUri);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ControlNotFoundException), ExpectedMessage = "Could not find link with link relation 'http://relations.restbucks.com/xyz'.")]
        public void ThrowsExceptionIfLinkCannotBeFound()
        {
            var link = RestbucksLink.WithRel("http://relations.restbucks.com/xyz");
            link.GetLinkInfo(DummyResponse.CreateResponse());
        }

        [Test]
        public void ShouldReturnTrueIfLinkExists()
        {
            var link = RestbucksLink.WithRel(new Uri("http://relations.restbucks.com/rfq"));
            Assert.IsTrue(link.LinkExists(DummyResponse.CreateResponse()));
        }

        [Test]
        public void ShouldReturnFalseIfLinkDoesNotExist()
        {
            var link = RestbucksLink.WithRel(new Uri("http://relations.restbucks.com/xyz"));
            Assert.IsFalse(link.LinkExists(DummyResponse.CreateResponse()));
        }
    }
}
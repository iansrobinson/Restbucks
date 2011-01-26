using NUnit.Framework;
using Restbucks.MediaType;

namespace Tests.Restbucks.MediaType
{
    [TestFixture]
    public class LinkRelationTests
    {
        [Test]
        public void ShouldParseStringTokenIntoStringLinkRelation()
        {
            var linkRelation = LinkRelation.Parse("self", s => string.Empty);
            Assert.IsInstanceOf<StringLinkRelation>(linkRelation);
        }

        [Test]
        public void ShouldParseAbsoluteUriIntoUriLinkRelation()
        {
            var linkRelation = LinkRelation.Parse("http://relations.restbucks.com/order", s => string.Empty);
            Assert.IsInstanceOf<UriLinkRelation>(linkRelation);
        }

        [Test]
        public void ShouldParseCompactUriIntoCompactUriLinkRelation()
        {
            var linkRelation = LinkRelation.Parse("rb:order", s => "http://relations.restbucks.com/");
            Assert.IsInstanceOf<CompactUriLinkRelation>(linkRelation);
        }

        [Test]
        public void ShouldIncludeFragmentIdentifierFromReferenceInValueOfCompactUriLinkRelation()
        {
            var linkRelation = LinkRelation.Parse("rb:#order", s => "http://relations.restbucks.com/");
            Assert.AreEqual("http://relations.restbucks.com/#order", linkRelation.Value);
        }

        [Test]
        public void IfPrefixIsNotRecognizedShouldReturnUriLinkRelation()
        {
            var linkRelation = LinkRelation.Parse("st:order", s => null);

            Assert.IsInstanceOf<UriLinkRelation>(linkRelation);
            Assert.AreEqual("st:order", linkRelation.Value);
        }

        [Test]
        public void IfTokenIsNullShouldReturnEmptyStringLinkRelation()
        {
            var linkRelation = LinkRelation.Parse(null, s => null);
            Assert.IsInstanceOf<StringLinkRelation>(linkRelation);
        }
    }
}
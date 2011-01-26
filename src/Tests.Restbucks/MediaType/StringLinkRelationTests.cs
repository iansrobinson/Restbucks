using NUnit.Framework;
using Restbucks.MediaType;

namespace Tests.Restbucks.MediaType
{
    [TestFixture]
    public class StringLinkRelationTests
    {
        [Test]
        public void ValueShouldReturnToken()
        {
            var linkRelation = new StringLinkRelation("self");
            Assert.AreEqual("self", linkRelation.Value);
        }

        [Test]
        public void SerializableValueShouldReturnToken()
        {
            var linkRelation = new StringLinkRelation("self");
            Assert.AreEqual("self", linkRelation.SerializableValue);
        }
    }
}
using NUnit.Framework;
using Restbucks.Client.Keys;

namespace Tests.Restbucks.Client.Keys
{
    [TestFixture]
    public class KeyEqualityComparerTests
    {
        [Test]
        public void TwoStringKeysWithSameValuesAreEqual()
        {
            Assert.IsTrue(KeyEqualityComparer.Instance.Equals(new StringKey("key-name"), new StringKey("key-name")));
        }

        [Test]
        public void TwoStringKeysWithDifferentValuesAreNotEqual()
        {
            Assert.IsFalse(KeyEqualityComparer.Instance.Equals(new StringKey("key-name"), new StringKey("different-key-name")));
        }

        [Test]
        public void ShouldBeCaseInsensitive()
        {
            Assert.IsTrue(KeyEqualityComparer.Instance.Equals(new StringKey("key-name"), new StringKey("KEY-NAME")));
        }
    }
}
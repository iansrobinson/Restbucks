using System.Net;
using NUnit.Framework;
using Restbucks.Client.Http;

namespace Tests.Restbucks.Client.Http
{
    [TestFixture]
    public class HttpStatusCodeExtensionMethodsTests
    {
        [Test]
        public void Is2XXTests()
        {
            Assert.IsTrue(HttpStatusCode.OK.Is2XX());
            Assert.IsTrue(HttpStatusCode.Created.Is2XX());

            Assert.IsFalse(HttpStatusCode.Continue.Is2XX());
            Assert.IsFalse(HttpStatusCode.SeeOther.Is2XX());
        }
    }
}
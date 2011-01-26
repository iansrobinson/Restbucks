using System.Xml.Linq;
using NUnit.Framework;
using Restbucks.MediaType.Assemblers;

namespace Tests.Restbucks.MediaType.Assemblers
{
    [TestFixture]
    public class LinksAssemblerTests
    {
        private const string LinkRelationsNamespace = "http://relations.restbucks.com/";

        [Test]
        public void ShouldReturnNamespaceForPrefix()
        {
            var element = new XElement("root", new XAttribute(XNamespace.Xmlns + "rb", LinkRelationsNamespace));
            Assert.AreEqual(LinkRelationsNamespace, LinksAssembler.LookupNamespace(element).Invoke("rb"));
        }

        [Test]
        public void ShouldReturnNullIfNamespaceDoesNotExistForPrefix()
        {
            var element = new XElement("root", new XAttribute(XNamespace.Xmlns + "rb", LinkRelationsNamespace));
            Assert.IsNull(LinksAssembler.LookupNamespace(element).Invoke("st"));
        }
    }
}
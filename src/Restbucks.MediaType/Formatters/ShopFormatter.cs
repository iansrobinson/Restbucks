using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Restbucks.MediaType.Formatters
{
    public class ShopFormatter
    {
        private readonly Shop shop;

        public ShopFormatter(Shop shop)
        {
            this.shop = shop;
        }

        public XElement CreateXml()
        {
            var root = new XElement(Namespaces.ShopSchema + "shop",
                                    CreateNamespaceAttributes(shop.Links),
                                    new ItemsFormatter(shop).CreateXml(),
                                    new LinksFormatter(shop).CreateXml(),
                                    new FormsFormatter(shop).CreateXml());

            return root;
        }

        private static XAttribute[] CreateNamespaceAttributes(IEnumerable<Link> links)
        {
            return (from LinkRelation rel in
                        (from Link link in links
                         select link.Rels).SelectMany(x => x)
                    where rel.GetType().Equals(typeof (CompactUriLinkRelation))
                    let linkRelation = (CompactUriLinkRelation)rel
                    select new {linkRelation.Prefix, NamespaceName = linkRelation.Uri.AbsoluteUri})
                .Distinct()
                .Select(value =>
                        new XAttribute(XNamespace.Xmlns + value.Prefix, value.NamespaceName))
                .ToArray();
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Restbucks.MediaType.Formatters
{
    public class LinksFormatter
    {
        private readonly Shop shop;

        public LinksFormatter(Shop shop)
        {
            this.shop = shop;
        }

        public IEnumerable<XElement> CreateXml()
        {
            return shop.Links.Select(link => new XElement(Namespaces.ShopSchema + "link",
                                                          new XAttribute("rel", ConcatenateLinkRelationValues(link)),
                                                          link.MediaType != null ? new XAttribute("type", link.MediaType) : null,
                                                          new XAttribute("href", link.Href)));
        }

        private static string ConcatenateLinkRelationValues(Link link)
        {
            var stringBuilder = link.Rels.Aggregate(
                new StringBuilder(), 
                (builder, rel) => builder.Append(" ").Append(rel.SerializableValue));
            return stringBuilder.ToString().Trim();
        }
    }
}
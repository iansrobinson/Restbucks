using System;
using System.Linq;
using System.Xml.Linq;

namespace Restbucks.MediaType.Assemblers
{
    public class ShopAssembler
    {
        private readonly XElement root;
        private readonly Uri parentBaseUri;

        public ShopAssembler(XElement root) : this(root, null)
        {
        }

        public ShopAssembler(XElement root, Uri parentBaseUri)
        {
            this.root = root;
            this.parentBaseUri = parentBaseUri;
        }

        public Shop AssembleShop()
        {
            if (!root.Name.Equals(Namespaces.ShopSchema + "shop"))
            {
                return null;
            }

            var baseUri = GetBaseUri(root, parentBaseUri);

            var shop = new Shop(baseUri);

            new ItemsAssembler(root).AssembleItems().ToList().ForEach(item => shop.AddItem(item));
            new LinksAssembler(root).AssembleLinks().ToList().ForEach(link => shop.AddLink(link));
            new FormsAssembler(root, baseUri).AssembleForms().ToList().ForEach(form => shop.AddForm(form));

            if (baseUri == null)
            {
                var relativeUriCount = (from l in shop.Links
                                        where l.Href.IsAbsoluteUri.Equals(false)
                                        select l.Href)
                    .Union(from f in shop.Forms
                           where f.Resource.IsAbsoluteUri.Equals(false)
                           select f.Resource).Count();

                if (relativeUriCount > 0)
                {
                    throw new InvalidFormatException("Invalid format. Base URI missing.");
                }
            }

            return shop;
        }

        private static Uri GetBaseUri(XElement element, Uri parentBaseUri)
        {
            var xmlBase = element.Attribute(XNamespace.Xml + "base");
            if (xmlBase != null)
            {
                return new Uri(xmlBase.Value);
            }

            return parentBaseUri;
        }
    }
}
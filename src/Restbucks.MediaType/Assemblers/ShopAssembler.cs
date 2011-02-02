using System;
using System.Linq;
using System.Xml.Linq;

namespace Restbucks.MediaType.Assemblers
{
    public class ShopAssembler
    {
        private readonly XElement root;
        private readonly Uri requestUri;

        public ShopAssembler(XElement root, Uri requestUri)
        {
            this.root = root;
            this.requestUri = requestUri;
        }

        public Shop AssembleShop()
        {
            if (!root.Name.Equals(Namespaces.ShopSchema + "shop"))
            {
                return null;
            }

            var shop = new Shop(GetBaseUri(root, requestUri));

            new ItemsAssembler(root).AssembleItems().ToList().ForEach(item => shop.AddItem(item));
            new LinksAssembler(root).AssembleLinks().ToList().ForEach(link => shop.AddLink(link));
            new FormsAssembler(root, requestUri).AssembleForms().ToList().ForEach(form => shop.AddForm(form));

            return shop;
        }

        private static Uri GetBaseUri(XElement element, Uri fallbackUri)
        {
            var xmlBase = element.Attribute(XNamespace.Xml + "base");
            if (xmlBase != null)
            {
                return new Uri(xmlBase.Value);
            }
            
            return fallbackUri;
        }
    }
}
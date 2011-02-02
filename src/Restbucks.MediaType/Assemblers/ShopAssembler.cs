using System;
using System.Linq;
using System.Xml.Linq;

namespace Restbucks.MediaType.Assemblers
{
    public class ShopAssembler
    {
        private readonly XElement root;

        public ShopAssembler(XElement root)
        {
            this.root = root;
        }

        public Shop AssembleShop()
        {
            if (!root.Name.Equals(Namespaces.ShopSchema + "shop"))
            {
                return null;
            }

            var shop = new Shop(new Uri("http://iansrobinson.com"));

            new ItemsAssembler(root).AssembleItems().ToList().ForEach(item => shop.AddItem(item));
            new LinksAssembler(root).AssembleLinks().ToList().ForEach(link => shop.AddLink(link));
            new FormsAssembler(root).AssembleForms().ToList().ForEach(form => shop.AddForm(form));

            return shop;
        }
    }
}
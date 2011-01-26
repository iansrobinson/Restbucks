using System.Xml.Linq;

namespace Restbucks.MediaType.Formatters
{
    public class ItemsFormatter
    {
        private readonly Shop shop;

        public ItemsFormatter(Shop shop)
        {
            this.shop = shop;
        }

        public XElement CreateXml()
        {
            if (!shop.HasItems)
            {
                return null;
            }
            var items = new XElement(Namespaces.ShopSchema + "items");
            foreach (var item in shop.Items)
            {
                items.Add(new XElement(Namespaces.ShopSchema + "item",
                                       new XElement(Namespaces.ShopSchema + "description", item.Description),
                                       new XElement(Namespaces.ShopSchema + "amount", new XAttribute("measure", item.Amount.Measure), item.Amount.Value),
                                       item.Cost != null ? new XElement(Namespaces.ShopSchema + "price", new XAttribute("currency", item.Cost.Currency), item.Cost.Value.ToString("F2")) : null));
            }
            return items;
        }
    }
}
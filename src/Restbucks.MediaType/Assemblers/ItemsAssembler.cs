using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Restbucks.MediaType.Assemblers
{
    public class ItemsAssembler
    {
        private readonly XElement root;

        public ItemsAssembler(XElement root)
        {
            this.root = root;
        }

        public IEnumerable<Item> AssembleItems()
        {
            var items = root.Descendants(Namespaces.ShopSchema + "item");
            return from itm in items
                   let description = itm.Element(Namespaces.ShopSchema + "description")
                   let amount = itm.Element(Namespaces.ShopSchema + "amount")
                   let price = itm.Element(Namespaces.ShopSchema + "price")
                   select new Item(
                       description.ApplyNonEmptyValueOrDefault(),
                       amount
                           .ApplyNonNullInstance(a => new Amount(
                                                          amount.Attribute("measure")
                                                              .ApplyNonEmptyValueOrDefault(),
                                                          amount
                                                              .ApplyNonEmptyValueOrDefault(value => Convert.ToInt32(value), 0))
                           ),
                       price
                           .ApplyNonNullInstance(p => new Cost(
                                                          price.Attribute("currency")
                                                              .ApplyNonEmptyValueOrDefault(),
                                                          price
                                                              .ApplyNonEmptyValueOrDefault(value => Convert.ToDouble(value), 0.00))
                           ));
        }
    }
}
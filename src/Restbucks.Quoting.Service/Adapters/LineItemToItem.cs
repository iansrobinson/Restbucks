using Restbucks.MediaType;

namespace Restbucks.Quoting.Service.Adapters
{
    public class LineItemToItem
    {
        private readonly LineItem lineItem;

        public LineItemToItem(LineItem lineItem)
        {
            this.lineItem = lineItem;
        }

        public Item Adapt()
        {
            return new Item(lineItem.Description,
                            new QuantityToAmount(lineItem.Quantity).Adapt(),
                            new MoneyToCost(lineItem.Price).Adapt());
        }
    }
}
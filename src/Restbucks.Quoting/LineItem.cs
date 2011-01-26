namespace Restbucks.Quoting
{
    public class LineItem
    {
        private readonly string description;
        private readonly Quantity quantity;
        private readonly Money price;

        public LineItem(string description, Quantity quantity, Money price)
        {
            this.description = description;
            this.quantity = quantity;
            this.price = price;
        }

        public string Description
        {
            get { return description; }
        }

        public Quantity Quantity
        {
            get { return quantity; }
        }

        public Money Price
        {
            get { return price; }
        }
    }
}
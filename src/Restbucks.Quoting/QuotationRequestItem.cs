namespace Restbucks.Quoting
{
    public class QuotationRequestItem
    {
        private readonly string description;
        private readonly Quantity quantity;

        public QuotationRequestItem(string description, Quantity quantity)
        {
            this.description = description;
            this.quantity = quantity;
        }

        public string Description
        {
            get { return description; }
        }

        public Quantity Quantity
        {
            get { return quantity; }
        }

        public LineItem CreateLineItem(Money price)
        {
            return new LineItem(description, quantity, price);
        }
    }
}
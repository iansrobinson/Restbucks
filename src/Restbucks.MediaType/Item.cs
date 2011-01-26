namespace Restbucks.MediaType
{
    public class Item
    {
        private readonly string description;
        private readonly Amount amount;
        private readonly Cost cost;

         public Item(string description, Amount amount, Cost cost)
        {
            this.description = description;
            this.amount = amount;
            this.cost = cost;
        }

        public Item(string description, Amount amount) : this(description, amount, null)
        {
        }

        public string Description
        {
            get { return description; }
        }

        public Amount Amount
        {
            get { return amount; }
        }

        public Cost Cost
        {
            get { return cost; }
        }
    }
}
namespace Restbucks.MediaType
{
    public class Cost
    {
        private readonly string currency;
        private readonly double value;

        public Cost(string currency, double value)
        {
            this.currency = currency;
            this.value = value;
        }

        public string Currency
        {
            get { return currency; }
        }

        public double Value
        {
            get { return value; }
        }
    }
}
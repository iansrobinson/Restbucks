namespace Restbucks.Quoting
{
    public struct Money
    {
        private readonly string currency;
        private readonly double value;

        public Money(string currency, double value)
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
namespace Restbucks.MediaType
{
    public class Amount
    {
        private readonly string measure;
        private readonly int value;

        public Amount(string measure, int value)
        {
            this.measure = measure;
            this.value = value;
        }

        public string Measure
        {
            get { return measure; }
        }

        public int Value
        {
            get { return value; }
        }
    }
}
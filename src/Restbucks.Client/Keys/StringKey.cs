namespace Restbucks.Client.Keys
{
    public class StringKey : IKey
    {
        private readonly string value;

        public StringKey(string value)
        {
            this.value = value;
        }

        public string Value
        {
            get { return value; }
        }
    }
}
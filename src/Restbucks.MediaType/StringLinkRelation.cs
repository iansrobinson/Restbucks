namespace Restbucks.MediaType
{
    public class StringLinkRelation : LinkRelation
    {
        private readonly string value;

        public StringLinkRelation(string value)
        {
            this.value = value;
        }

        public override string Value
        {
            get { return value; }
        }

        public override string SerializableValue
        {
            get { return value; }
        }
    }
}
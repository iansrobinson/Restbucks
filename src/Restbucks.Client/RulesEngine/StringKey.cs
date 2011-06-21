using Restbucks.RestToolkit.Utils;

namespace Restbucks.Client.RulesEngine
{
    public class StringKey : IKey
    {
        private readonly string value;

        public StringKey(string value)
        {
            CheckString.Is(Not.NullOrEmptyOrWhitespace, value, "value");
            this.value = value;
        }

        public override string ToString()
        {
            return value;
        }

        public bool Equals(StringKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.value, value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (StringKey)) return false;
            return Equals((StringKey) obj);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
    }
}
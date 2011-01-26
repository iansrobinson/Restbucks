using System;

namespace Restbucks.MediaType
{
    public static class Check
    {
        public static void IsNotNull(object o, string name)
        {
            if (o == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        public static void IsNotNullOrWhitespace(string s, string name)
        {
            if (s == null)
            {
                throw new ArgumentNullException(name);
            }

            if (String.IsNullOrWhiteSpace(s))
            {
                throw new ArgumentException("Value cannot be empty or whitespace.", name);
            }
        }
    }
}
using System;

namespace Restbucks.MediaType
{
    [Flags]
    public enum Not
    {
        Null = 0,
        Empty = 1,
        Whitespace =2,
        NullOrEmptyOrWhitespace = Null | Empty | Whitespace
    }
    
    public static class CheckString
    {
        public static void Is(Not values, string s, string name)
        {
            if ((values & Not.Null) == Not.Null)
            {
                if (s == null)
                {
                    throw new ArgumentNullException(name);
                }
            }

            if ((values & Not.Empty) == Not.Empty)
            {
                if (string.IsNullOrEmpty(s))
                {
                    throw new ArgumentException("Value cannot be empty.", name);
                }
            }

            if ((values & Not.Whitespace) == Not.Whitespace)
            {
                if ((s.Length > 0) && (s.Trim().Length == 0))
                {
                    throw new ArgumentException("Value cannot be whitespace.", name);
                }
            }
        }
    }
}
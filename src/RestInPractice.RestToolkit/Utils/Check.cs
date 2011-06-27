using System;

namespace RestInPractice.RestToolkit.Utils
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
    }
}
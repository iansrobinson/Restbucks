using System;
using System.Collections.Generic;
using Restbucks.Client.Keys;

namespace Restbucks.Client
{
    public class ApplicationContext
    {
        private readonly Dictionary<IKey, object> values;

        public ApplicationContext()
        {
            values = new Dictionary<IKey, object>(KeyEqualityComparer.Instance);
        }

        public void Set<T>(IKey key, T value)
        {
            if (values.ContainsKey(key))
            {
                if (!values[key].GetType().Equals(typeof(T)))
                {
                    throw new InvalidOperationException("Unable to replace an existing value with a value of a different type.");
                }
                values[key] = value;
            }
            else
            {
                values.Add(key, value);
            } 
        }

        public T Get<T>(IKey key)
        {
            return (T) values[key];
        }
    }
}
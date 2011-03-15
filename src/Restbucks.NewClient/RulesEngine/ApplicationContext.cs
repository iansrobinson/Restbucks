using System;
using System.Collections.Generic;
using System.Linq;

namespace Restbucks.NewClient.RulesEngine
{
    public class ApplicationContext
    {
        private readonly IDictionary<IKey, object> values;

        public ApplicationContext(params KeyValuePair<IKey, object>[] values)
        {
            this.values = new Dictionary<IKey, object>(values.Length);
            values.ToList().ForEach(kv => this.values.Add(kv));
        }

        public T Get<T>(IKey key)
        {
            return (T) values[key];
        }

        public bool ContainsKey(IKey key)
        {
            return values.ContainsKey(key);
        }

        public IApplicationContextBuilder GetNewContextBuilder()
        {
            return new ApplicationContextBuilder(values.ToArray());
        }

        private class ApplicationContextBuilder : IApplicationContextBuilder
        {
            private readonly IDictionary<IKey, object> values;

            public ApplicationContextBuilder(params KeyValuePair<IKey, object>[] values)
            {
                this.values = new Dictionary<IKey, object>(values.Length);
                values.ToList().ForEach(kv => this.values.Add(kv));
            }

            public IApplicationContextBuilder Add(IKey key, object value)
            {
                values.Add(key, value);
                return this;
            }

            public IApplicationContextBuilder Remove(IKey key)
            {
                values.Remove(key);
                return this;
            }

            public IApplicationContextBuilder Update(IKey key, object value)
            {
                if (values.ContainsKey(key))
                {
                    if (!values[key].GetType().Equals(value.GetType()))
                    {
                        throw new InvalidOperationException("Unable to replace an existing value with a value of a different type.");
                    }
                    values[key] = value;
                }
                else
                {
                    values.Add(key, value);
                }
                return this;
            }

            public ApplicationContext Build()
            {
                return new ApplicationContext(values.ToArray());
            }
        }
    }
}
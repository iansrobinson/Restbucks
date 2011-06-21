using System;
using System.Collections.Generic;
using System.Linq;

namespace Restbucks.RestToolkit.RulesEngine
{
    public class ApplicationStateVariables
    {
        private readonly IDictionary<IKey, object> values;

        public ApplicationStateVariables(params KeyValuePair<IKey, object>[] values)
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

        public IApplicationStateVariablesBuilder GetNewStateVariablesBuilder()
        {
            return new ApplicationStateVariablesBuilder(values.ToArray());
        }

        private class ApplicationStateVariablesBuilder : IApplicationStateVariablesBuilder
        {
            private readonly IDictionary<IKey, object> values;

            public ApplicationStateVariablesBuilder(params KeyValuePair<IKey, object>[] values)
            {
                this.values = new Dictionary<IKey, object>(values.Length);
                values.ToList().ForEach(kv => this.values.Add(kv));
            }

            public IApplicationStateVariablesBuilder Add(IKey key, object value)
            {
                values.Add(key, value);
                return this;
            }

            public IApplicationStateVariablesBuilder Remove(IKey key)
            {
                values.Remove(key);
                return this;
            }

            public IApplicationStateVariablesBuilder Update(IKey key, object value)
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

            public ApplicationStateVariables Build()
            {
                return new ApplicationStateVariables(values.ToArray());
            }
        }
    }
}
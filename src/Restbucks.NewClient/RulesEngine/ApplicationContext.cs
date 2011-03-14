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

        public object Input
        {
            get { return null; }
        }

        public T Get<T>(IKey key)
        {
            return (T) values[key];
        }
    }
}
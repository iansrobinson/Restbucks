using System.Collections.Generic;
using System.Linq;

namespace Restbucks.NewClient.RulesEngine
{
    public class ApplicationContext
    {
        public static ApplicationContextBuilder GetBuilder(ApplicationContext context)
        {
            return new ApplicationContextBuilder(context.values.ToArray());
        }

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

        public class ApplicationContextBuilder
        {
            private readonly IDictionary<IKey, object> values;

            public ApplicationContextBuilder(params KeyValuePair<IKey, object>[] values)
            {
                this.values = new Dictionary<IKey, object>(values.Length);
                values.ToList().ForEach(kv => this.values.Add(kv));
            }

            public ApplicationContextBuilder Add(IKey key, object value)
            {
                values.Add(key, value);
                return this;
            }

            public ApplicationContext Build()
            {
                return new ApplicationContext(values.ToArray());
            }
        }
    }
}
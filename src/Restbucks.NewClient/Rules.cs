using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Restbucks.NewClient
{
    public class Rules
    {
        private readonly IEnumerable<IRule> rules;

        public Rules(params IRule[] rules)
        {
            this.rules = rules;
        }

        public void Evaluate(HttpResponseMessage response)
        {
            rules.ToList().ForEach(r => r.Evaluate(response));
        }
    }
}
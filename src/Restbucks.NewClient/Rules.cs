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

        public IState Evaluate(HttpResponseMessage response)
        {
            return (from rule in rules
                    select rule.Evaluate(response)
                    into result
                    where result.IsSuccessful
                    select result.State).FirstOrDefault();
        }
    }
}
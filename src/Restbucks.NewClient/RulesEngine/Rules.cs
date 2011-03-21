using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public class Rules
    {
        private readonly IEnumerable<IRule> rules;

        public Rules(params IRule[] rules)
        {
            this.rules = rules;
        }

        public IState Evaluate(HttpResponseMessage previousResponse, ApplicationStateVariables stateVariables)
        {
            return (from rule in rules
                    select rule.Evaluate(previousResponse, stateVariables)
                    into result
                    where result.IsSuccessful
                    select result.State).FirstOrDefault();
        }
    }
}
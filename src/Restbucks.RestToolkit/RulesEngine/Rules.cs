using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Restbucks.RestToolkit.RulesEngine
{
    public class Rules
    {
        private readonly IEnumerable<IRule> rules;

        public Rules(params IRule[] rules)
        {
            this.rules = rules;
        }

        public IState Evaluate(HttpResponseMessage previousResponse, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities)
        {
            return (from rule in rules
                    select rule.Evaluate(previousResponse, stateVariables, clientCapabilities)
                    into result
                    where result.IsSuccessful
                    select result.State).FirstOrDefault();
        }
    }
}
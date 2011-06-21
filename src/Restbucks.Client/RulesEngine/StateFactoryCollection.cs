using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Restbucks.Client.RulesEngine
{
    public class StateFactoryCollection
    {
        private readonly IEnumerable<StateCreationRule> rules;
        private readonly CreateStateDelegate createDefaultState;

        private static readonly CreateStateDelegate CreateUnsuccessfulState = (r, v, c) => UnsuccessfulState.Instance;

        public StateFactoryCollection(IEnumerable<StateCreationRule> rules) : this(rules, CreateUnsuccessfulState)
        {
        }

        public StateFactoryCollection(IEnumerable<StateCreationRule> rules, CreateStateDelegate createDefaultState)
        {
            this.rules = rules;
            this.createDefaultState = createDefaultState;
        }

        public IState Create(HttpResponseMessage response, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities)
        {
            foreach (var result in rules.Select(rule => rule.Evaluate(response, stateVariables, clientCapabilities)).Where(result => result.IsSuccessful))
            {
                return result.State;
            }
            return createDefaultState(response, stateVariables, clientCapabilities);
        }
    }
}
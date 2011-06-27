using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace RestInPractice.RestToolkit.RulesEngine
{
    public class StateFactoryCollection : ICreateNextState
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

        public IState Execute(HttpResponseMessage response, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities)
        {
            foreach (var result in rules.Select(rule => rule.Evaluate(response, stateVariables, clientCapabilities)).Where(result => result.IsSuccessful))
            {
                return result.State;
            }
            return createDefaultState(response, stateVariables, clientCapabilities);
        }
    }
}
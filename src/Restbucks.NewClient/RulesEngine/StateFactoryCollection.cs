using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public class StateFactoryCollection : IStateFactory
    {
        private readonly IEnumerable<StateCreationRule> rules;
        private readonly IStateFactory defaultFactory;

        public StateFactoryCollection(IEnumerable<StateCreationRule> rules) : this(rules, UnsuccessfulStateFactory.Instance)
        {
        }

        public StateFactoryCollection(IEnumerable<StateCreationRule> rules, IStateFactory defaultFactory)
        {
            this.rules = rules;
            this.defaultFactory = defaultFactory;
        }

        public IState Create(HttpResponseMessage response, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities)
        {
            foreach (var result in rules.Select(rule => rule.Evaluate(response, stateVariables, clientCapabilities)).Where(result => result.IsSuccessful))
            {
                return result.State;
            }
            return defaultFactory.Create(response, stateVariables, clientCapabilities);
        }

        private class UnsuccessfulStateFactory : IStateFactory
        {
            public static readonly IStateFactory Instance = new UnsuccessfulStateFactory();

            private UnsuccessfulStateFactory()
            {
            }

            public IState Create(HttpResponseMessage response, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities)
            {
                return UnsuccessfulState.Instance;
            }
        }
    }
}
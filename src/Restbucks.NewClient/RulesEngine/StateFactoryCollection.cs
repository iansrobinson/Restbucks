using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public class StateFactoryCollection : IStateFactory
    {
        private readonly IEnumerable<StateCreationRule> rules;
        private readonly IDictionary<HttpStatusCode, IStateFactory> factoryWorkers;
        private readonly IStateFactory defaultFactory;

        public StateFactoryCollection(IEnumerable<StateCreationRule> rules) : this(rules, UnsuccessfulStateFactory.Instance)
        {
        }

        public StateFactoryCollection(IEnumerable<StateCreationRule> rules, IStateFactory defaultFactory)
        {
            this.rules = rules;
            this.defaultFactory = defaultFactory;
        }

        public StateFactoryCollection(IDictionary<HttpStatusCode, IStateFactory> factoryWorkers) : this(factoryWorkers, UnsuccessfulStateFactory.Instance)
        {
        }

        public StateFactoryCollection(IDictionary<HttpStatusCode, IStateFactory> factoryWorkers, IStateFactory defaultFactory)
        {
            this.factoryWorkers = factoryWorkers;
            this.defaultFactory = defaultFactory;
        }

        public IState Create(HttpResponseMessage response, ApplicationContext context)
        {
            if (factoryWorkers.ContainsKey(response.StatusCode))
            {
                return factoryWorkers[response.StatusCode].Create(response, context);
            }
            return defaultFactory.Create(response, context);
        }

        public IState CreateState(HttpResponseMessage response, ApplicationContext context)
        {
            foreach (var result in rules.Select(rule => rule.Evaluate(response, context)).Where(result => result.IsSuccessful))
            {
                return result.State;
            }
            return defaultFactory.Create(response, context);
        }

        private class UnsuccessfulStateFactory : IStateFactory
        {
            public static readonly IStateFactory Instance = new UnsuccessfulStateFactory();

            private UnsuccessfulStateFactory()
            {
            }

            public IState Create(HttpResponseMessage response, ApplicationContext context)
            {
                return UnsuccessfulState.Instance;
            }
        }
    }
}
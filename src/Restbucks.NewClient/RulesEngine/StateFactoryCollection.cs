using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public class StateFactoryCollection : IStateFactory
    {
        private readonly IDictionary<HttpStatusCode, IStateFactory> factoryWorkers;
        private readonly IStateFactory defaultWorker;

        public StateFactoryCollection(IDictionary<HttpStatusCode, IStateFactory> factoryWorkers) : this(factoryWorkers, UnsuccessfulStateFactory.Instance)
        {
        }

        public StateFactoryCollection(IDictionary<HttpStatusCode, IStateFactory> factoryWorkers, IStateFactory defaultWorker)
        {
            this.factoryWorkers = factoryWorkers;
            this.defaultWorker = defaultWorker;
        }

        public IState Create(HttpResponseMessage response)
        {
            if (factoryWorkers.ContainsKey(response.StatusCode))
            {
                return factoryWorkers[response.StatusCode].Create(response);
            }
            return defaultWorker.Create(response);
        }

        private class UnsuccessfulStateFactory : IStateFactory
        {
            public static readonly IStateFactory Instance = new UnsuccessfulStateFactory();

            private UnsuccessfulStateFactory()
            {
            }

            public IState Create(HttpResponseMessage response)
            {
                return UnsuccessfulState.Instance;
            }
        }
    }
}
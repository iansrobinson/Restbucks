using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public class StateFactory : IStateFactory
    {
        private readonly CreateNextStateDelegate createNextStateDelegate;

        public StateFactory(CreateNextStateDelegate createNextStateDelegate)
        {
            this.createNextStateDelegate = createNextStateDelegate;
        }

        public IState Create(HttpResponseMessage response, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities)
        {
            return createNextStateDelegate(response, stateVariables);
        }
    }
}
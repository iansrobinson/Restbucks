using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public class StateFactory : IStateFactory
    {
        private readonly StateDelegate stateDelegate;

        public StateFactory(StateDelegate stateDelegate)
        {
            this.stateDelegate = stateDelegate;
        }

        public IState Create(HttpResponseMessage response, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities)
        {
            return stateDelegate(response, stateVariables);
        }
    }
}
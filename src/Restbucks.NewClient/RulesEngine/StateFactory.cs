using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public class StateFactory : IStateFactory
    {
        private readonly CreateStateDelegate createStateDelegate;

        public StateFactory(CreateStateDelegate createStateDelegate)
        {
            this.createStateDelegate = createStateDelegate;
        }

        public IState Create(HttpResponseMessage response, ApplicationStateVariables stateVariables)
        {
            return createStateDelegate(response, stateVariables);
        }
    }
}
using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public class StateFactory : IStateFactory
    {
        private readonly CreateState createState;

        public StateFactory(CreateState createState)
        {
            this.createState = createState;
        }

        public IState Create(HttpResponseMessage response, ApplicationContext context, Actions actions)
        {
            return createState(response, context, actions);
        }
    }
}
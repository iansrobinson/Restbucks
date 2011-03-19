using System;
using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public class StateFactory : IStateFactory
    {
        private readonly Func<HttpResponseMessage, ApplicationContext, Actions, IState> createState;

        public StateFactory(Func<HttpResponseMessage, ApplicationContext, Actions, IState> createState)
        {
            this.createState = createState;
        }

        public IState Create(HttpResponseMessage response, ApplicationContext context, Actions actions)
        {
            return createState(response, context, actions);
        }
    }
}
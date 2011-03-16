using System;
using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public class StateFactory : IStateFactory
    {
        private readonly Func<HttpResponseMessage, ApplicationContext, IState> createState;

        public StateFactory(Func<HttpResponseMessage, ApplicationContext, IState> createState)
        {
            this.createState = createState;
        }

        public IState Create(HttpResponseMessage response, ApplicationContext context)
        {
            return createState(response, context);
        }
    }
}
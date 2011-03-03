using System;
using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public class StateFactory : IStateFactory
    {
        private readonly Func<HttpResponseMessage, IState> createState;

        public StateFactory(Func<HttpResponseMessage, IState> createState)
        {
            this.createState = createState;
        }

        public IState Create(HttpResponseMessage response)
        {
            return createState(response);
        }
    }
}
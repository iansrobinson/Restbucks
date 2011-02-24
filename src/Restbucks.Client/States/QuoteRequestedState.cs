using System;
using System.Net.Http;

namespace Restbucks.Client.States
{
    public class QuoteRequestedState : IState
    {
        private readonly HttpResponseMessage response;
        private readonly ApplicationContext context;

        public QuoteRequestedState(HttpResponseMessage response, ApplicationContext context)
        {
            this.response = response;
            this.context = context;
        }

        public IState Apply(IResponseHandlers handlers)
        {
            throw new NotImplementedException();
        }

        public bool IsTerminalState
        {
            get { return true; }
        }
    }
}
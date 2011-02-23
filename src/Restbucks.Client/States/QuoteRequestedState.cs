using System;
using System.Net.Http;
using Restbucks.Client.ResponseHandlers;

namespace Restbucks.Client.States
{
    public class QuoteRequestedState : IState
    {
        private readonly IResponseHandlerProvider responseHandlers;
        private readonly ApplicationContext context;
        private readonly HttpResponseMessage response;

        public QuoteRequestedState(IResponseHandlerProvider responseHandlers, ApplicationContext context, HttpResponseMessage response)
        {
            this.responseHandlers = responseHandlers;
            this.context = context;
            this.response = response;
        }

        public IState Apply()
        {
            throw new NotImplementedException();
        }

        public bool IsTerminalState
        {
            get { return true; }
        }
    }
}
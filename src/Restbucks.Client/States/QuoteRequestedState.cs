using System;
using System.Net.Http;
using Restbucks.Client.ResponseHandlers;

namespace Restbucks.Client.States
{
    public class QuoteRequestedState : IState
    {
        private readonly HttpResponseMessage response;
        private readonly ApplicationContext context;
        private readonly IHttpClientProvider clientProvider;

        public QuoteRequestedState(HttpResponseMessage response, ApplicationContext context, IHttpClientProvider clientProvider)
        {
            this.response = response;
            this.context = context;
            this.clientProvider = clientProvider;
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
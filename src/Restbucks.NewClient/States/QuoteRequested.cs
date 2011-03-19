using System;
using System.Net.Http;
using Restbucks.NewClient.RulesEngine;

namespace Restbucks.NewClient.States
{
    public class QuoteRequested : IState
    {
        private readonly HttpResponseMessage previousResponse;
        private readonly ApplicationContext context;

        public QuoteRequested(HttpResponseMessage previousResponse, ApplicationContext context)
        {
            this.previousResponse = previousResponse;
            this.context = context;
        }

        public IState NextState(Actions actions)
        {
            throw new NotImplementedException();
        }

        public bool IsTerminalState
        {
            get { return true; }
        }
    }
}
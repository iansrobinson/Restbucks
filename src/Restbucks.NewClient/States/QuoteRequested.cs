using System;
using System.Net.Http;
using Restbucks.NewClient.RulesEngine;

namespace Restbucks.NewClient.States
{
    public class QuoteRequested : IState
    {
        private readonly HttpResponseMessage previousResponse;
        private readonly ApplicationContext context;
        private readonly Actions actions;

        public QuoteRequested(HttpResponseMessage previousResponse, ApplicationContext context, Actions actions)
        {
            this.previousResponse = previousResponse;
            this.context = context;
            this.actions = actions;
        }

        public IState NextState()
        {
            throw new NotImplementedException();
        }

        public bool IsTerminalState
        {
            get { return true; }
        }
    }
}
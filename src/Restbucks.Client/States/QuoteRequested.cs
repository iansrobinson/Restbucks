using System;
using System.Net.Http;
using Restbucks.Client.RulesEngine;

namespace Restbucks.Client.States
{
    public class QuoteRequested : IState
    {
        private readonly HttpResponseMessage previousResponse;
        private readonly ApplicationStateVariables stateVariables;

        public QuoteRequested(HttpResponseMessage previousResponse, ApplicationStateVariables stateVariables)
        {
            this.previousResponse = previousResponse;
            this.stateVariables = stateVariables;
        }

        public IState NextState(IClientCapabilities clientCapabilities)
        {
            throw new NotImplementedException();
        }

        public bool IsTerminalState
        {
            get { return true; }
        }
    }
}
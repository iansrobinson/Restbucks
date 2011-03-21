﻿using System;
using System.Net.Http;
using Restbucks.NewClient.RulesEngine;

namespace Restbucks.NewClient.States
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
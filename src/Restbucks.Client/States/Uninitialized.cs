﻿using System;
using System.Net.Http;
using RestInPractice.RestToolkit.RulesEngine;

namespace Restbucks.Client.States
{
    public class Uninitialized : IState
    {
        private readonly ApplicationStateVariables stateVariables;

        public Uninitialized(ApplicationStateVariables stateVariables)
        {
            this.stateVariables = stateVariables;
        }

        public IState NextState(IClientCapabilities clientCapabilities)
        {
            var rules = new Rules(
                When.IsTrue(response => true)
                    .Execute(actions => actions.Do(GetHomePage.Instance))
                    .ReturnState((response, vars) => new Started(response, vars)));

            return rules.Evaluate(null, stateVariables, clientCapabilities);
        }

        public bool IsTerminalState
        {
            get { return false; }
        }

        private class GetHomePage : IRequestAction
        {
            public static readonly IRequestAction Instance = new GetHomePage();

            private GetHomePage()
            {
            }

            public HttpResponseMessage Execute(HttpResponseMessage previousResponse, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities)
            {
                return clientCapabilities.GetHttpClient().Get(stateVariables.Get<Uri>(new StringKey("home-page-uri")));
            }
        }
    }
}
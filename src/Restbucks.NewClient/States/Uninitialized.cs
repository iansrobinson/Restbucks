using System;
using System.Net.Http;
using Restbucks.NewClient.RulesEngine;

namespace Restbucks.NewClient.States
{
    public class Uninitialized : IState
    {
        private readonly ApplicationStateVariables stateVariables;

        public Uninitialized(ApplicationStateVariables stateVariables)
        {
            this.stateVariables = stateVariables;
        }

        public IState NextState(Actions actions)
        {
            var rules = new Rules(
                When.IsTrue(response => true)
                    .ExecuteAction(actions.Do(GetHomePage.Instance))
                    .ReturnState((response, vars) => new Started(response, vars)));

            return rules.Evaluate(null, stateVariables);
        }

        public bool IsTerminalState
        {
            get { return false; }
        }

        private class GetHomePage : IAction
        {
            public static readonly IAction Instance = new GetHomePage();

            private GetHomePage()
            {
            }

            public HttpResponseMessage Execute(HttpResponseMessage response, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities)
            {
                return clientCapabilities.GetHttpClient().Get(stateVariables.Get<Uri>(new StringKey("home-page-uri")));
            }
        }
    }
}
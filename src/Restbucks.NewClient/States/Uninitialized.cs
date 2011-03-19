using System;
using System.Net.Http;
using Restbucks.NewClient.RulesEngine;

namespace Restbucks.NewClient.States
{
    public class Uninitialized : IState
    {
        private readonly ApplicationContext context;
        private readonly Actions actions;

        public Uninitialized(ApplicationContext context, Actions actions)
        {
            this.context = context;
            this.actions = actions;
        }

        public IState NextState()
        {
            var rules = new Rules(
                When.IsTrue(response => true)
                    .ExecuteAction(a => a.Do(GetHomePage.Instance))
                    .ReturnState((response, ctx, actns) => new Started(response, ctx, actns)));

            return rules.Evaluate(null, context, actions);
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

            public HttpResponseMessage Execute(HttpResponseMessage previousResponse, ApplicationContext context, IClientCapabilities clientCapabilities)
            {
                return clientCapabilities.GetHttpClient().Get(context.Get<Uri>(new StringKey("home-page-uri")));
            }
        }
    }
}
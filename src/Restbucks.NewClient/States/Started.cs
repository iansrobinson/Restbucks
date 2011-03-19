using System.Net;
using System.Net.Http;
using Restbucks.NewClient.RulesEngine;

namespace Restbucks.NewClient.States
{
    public class Started : IState
    {
        private readonly HttpResponseMessage previousResponse;
        private readonly ApplicationContext context;
        private readonly Actions actions;

        public Started(HttpResponseMessage previousResponse, ApplicationContext context, Actions actions)
        {
            this.previousResponse = previousResponse;
            this.context = context;
            this.actions = actions;
        }

        public IState NextState()
        {
            var rules = new Rules(
                When
                    .IsTrue(response => response.ContainsForm(Forms.RequestForQuote))
                    .ExecuteAction(a => a.SubmitForm(Forms.RequestForQuote))
                    .Return(
                        new[]
                            {
                                On.Status(HttpStatusCode.Created)
                                    .Do((response, ctx, actns)
                                        => new QuoteRequested(response, ctx, actns))
                            }
                    ),
                When
                    .IsTrue(response => response.ContainsLink(Links.Rfq))
                    .ExecuteAction(a => a.ClickLink(Links.Rfq))
                    .Return(
                        new[]
                            {
                                On.Status(HttpStatusCode.OK)
                                    .Do((response, ctx, actns)
                                        => new Started(response, ctx, actns))
                            })
                );

            return rules.Evaluate(previousResponse, context, actions);
        }

        public bool IsTerminalState
        {
            get { return false; }
        }
    }
}
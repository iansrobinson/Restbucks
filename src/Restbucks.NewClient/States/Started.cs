using System.Net;
using System.Net.Http;
using Restbucks.NewClient.RulesEngine;

namespace Restbucks.NewClient.States
{
    public class Started : IState
    {
        private readonly HttpResponseMessage previousResponse;
        private readonly ApplicationContext context;

        public Started(HttpResponseMessage previousResponse, ApplicationContext context)
        {
            this.previousResponse = previousResponse;
            this.context = context;
        }

        public IState NextState(Actions actions)
        {
            var rules = new Rules(
                When
                    .IsTrue(response => response.ContainsForm(Forms.RequestForQuote))
                    .ExecuteAction(actions.SubmitForm(Forms.RequestForQuote))
                    .Return(
                        new[]
                            {
                                On.Status(HttpStatusCode.Created)
                                    .Do((response, ctx, actns)
                                        => new QuoteRequested(response, ctx))
                            }
                    ),
                When
                    .IsTrue(response => response.ContainsLink(Links.Rfq))
                    .ExecuteAction(actions.ClickLink(Links.Rfq))
                    .Return(
                        new[]
                            {
                                On.Status(HttpStatusCode.OK)
                                    .Do((response, ctx, actns)
                                        => new Started(response, ctx))
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
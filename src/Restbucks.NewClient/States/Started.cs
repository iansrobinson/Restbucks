﻿using System.Net;
using System.Net.Http;
using Restbucks.NewClient.RulesEngine;

namespace Restbucks.NewClient.States
{
    public class Started : IState
    {
        private readonly HttpResponseMessage previousResponse;
        private readonly ApplicationStateVariables stateVariables;

        public Started(HttpResponseMessage previousResponse, ApplicationStateVariables stateVariables)
        {
            this.previousResponse = previousResponse;
            this.stateVariables = stateVariables;
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
                                    .Do((response, vars)
                                        => new QuoteRequested(response, vars))
                            }
                    ),
                When
                    .IsTrue(response => response.ContainsLink(Links.Rfq))
                    .ExecuteAction(actions.ClickLink(Links.Rfq))
                    .Return(
                        new[]
                            {
                                On.Status(HttpStatusCode.OK)
                                    .Do((response, vars)
                                        => new Started(response, vars))
                            })
                );

            return rules.Evaluate(previousResponse, stateVariables);
        }

        public bool IsTerminalState
        {
            get { return false; }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using Restbucks.Client.ResponseHandlers;
using Restbucks.Client.States;

namespace Restbucks.Client.RulesEngine
{
    public class Rules
    {
        private readonly IEnumerable<IRule> rules;

        public Rules(params IRule[] rules)
        {
            for (var i = 0; i < rules.Length - 1; i++)
            {
                if (rules[i].GetType().Equals(typeof (ElseRule)))
                {
                    throw new ArgumentException("ElseRule must be last rule in list.", "rules");
                }
            }

            this.rules = rules;
        }

        public IState Evaluate(IResponseHandlerProvider responseHandlers, ApplicationContext context, HttpResponseMessage response)
        {
            var getResponseHandler = responseHandlers.GetType().GetMethod("GetFor", BindingFlags.Instance | BindingFlags.Public);

            foreach (var rule in rules)
            {
                if (rule.IsApplicable)
                {
                    var result = rule.Evaluate(getResponseHandler, responseHandlers, response, context);
                    if (result.IsSuccessful)
                    {
                        return rule.CreateNewState(responseHandlers, context, result.Response);
                    }
                }
            }

            return new TerminalState();
        }
    }
}
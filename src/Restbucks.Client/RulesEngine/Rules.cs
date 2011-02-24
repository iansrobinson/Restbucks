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

        public IState Evaluate(HttpResponseMessage response, ApplicationContext context, IHttpClientProvider clientProvider)
        {
            foreach (var rule in rules)
            {
                var result = rule.Evaluate(response, context, clientProvider);
                if (result.IsSuccessful)
                {
                    return rule.CreateNewState(result.Response, context, clientProvider);
                }
            }

            return new TerminalState();
        }
    }
}
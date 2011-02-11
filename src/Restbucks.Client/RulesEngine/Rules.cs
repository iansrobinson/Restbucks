using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using Restbucks.Client.ResponseHandlers;

namespace Restbucks.Client.RulesEngine
{
    public class Rules
    {
        private readonly IEnumerable<Rule> rules;

        public Rules(params Rule[] rules)
        {
            this.rules = rules;
        }

        public IState Evaluate(IResponseHandlerProvider responseHandlers, ApplicationContext context, HttpResponseMessage response)
        {
            var getFor = responseHandlers.GetType().GetMethod("GetFor", BindingFlags.Instance | BindingFlags.Public);
            
            foreach (var rule in rules)
            {
                if (rule.IsApplicable)
                {
                    var genericMethod = getFor.MakeGenericMethod(new[] { rule.ResponseHandlerType });
                    var handler = genericMethod.Invoke(responseHandlers, null) as IResponseHandler;
                    var result = handler.Handle(response, context);
                    if (result.IsSuccessful)
                    {
                        context.Set(ApplicationContextKeys.ContextName, rule.ContextName);
                        return rule.CreateState(responseHandlers, context, result.Response);
                    }
                }
            }

            return null;
        }
    }
}
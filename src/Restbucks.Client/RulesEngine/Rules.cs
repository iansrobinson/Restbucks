using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Restbucks.Client.ResponseHandlers;

namespace Restbucks.Client.RulesEngine
{
    public class Rules
    {
        private readonly IEnumerable<IRule> rules;

        public Rules(params IRule[] rules)
        {
            var hasNonTerminalElseRules = rules.Where(
                (rule, index) => rule.GetType().Equals(typeof (ElseRule))
                                 && index < rules.GetUpperBound(0))
                                             .Count() > 0;

            if (hasNonTerminalElseRules)
            {
                throw new ArgumentException("ElseRule must be last rule in list.", "rules");
            }

            if (rules.LastOrDefault(rule => rule.GetType().Equals(typeof (ElseRule))) == null)
            {
                this.rules = rules.Concat(new[]{ Else.UpdateContext(c => { }).Terminate()});
            }
            else
            {
                this.rules = rules;
            } 
        }

        public IState Evaluate(IResponseHandlerProvider responseHandlers, ApplicationContext context, HttpResponseMessage response)
        {
            var getFor = responseHandlers.GetType().GetMethod("GetFor", BindingFlags.Instance | BindingFlags.Public);

            foreach (var rule in rules)
            {
                if (rule.IsApplicable)
                {
                    var genericMethod = getFor.MakeGenericMethod(new[] {rule.ResponseHandlerType});
                    IResponseHandler handler;
                    try
                    {
                        handler = genericMethod.Invoke(responseHandlers, null) as IResponseHandler;
                    }
                    catch (TargetInvocationException ex)
                    {
                        if (ex.InnerException.GetType().Equals(typeof (KeyNotFoundException)))
                        {
                            throw new ResponseHandlerMissingException(string.Format("Response handler missing. Type: [{0}].", rule.ResponseHandlerType.FullName));
                        }
                        throw;
                    }

                    var result = handler.Handle(response, context);

                    if (result.IsSuccessful)
                    {
                        rule.ContextAction(context);

                        var state = rule.CreateState(responseHandlers, context, result.Response);

                        if (state == null)
                        {
                            throw new NullStateException();
                        }

                        return state;
                    }
                }
            }

            return null;
        }
    }
}
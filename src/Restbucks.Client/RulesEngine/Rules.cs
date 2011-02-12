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
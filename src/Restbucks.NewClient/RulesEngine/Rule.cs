using System.Net.Http;
using Restbucks.RestToolkit.Utils;

namespace Restbucks.NewClient.RulesEngine
{
    public class Rule : IRule
    {
        private readonly ICondition condition;
        private readonly IActionInvoker actionInvoker;
        private readonly IStateFactory stateFactory;

        public Rule(ICondition condition, IActionInvoker actionInvoker, IStateFactory stateFactory)
        {
            Check.IsNotNull(condition, "condition");
            Check.IsNotNull(actionInvoker, "actionInvoker");
            Check.IsNotNull(stateFactory, "stateFactory");

            this.condition = condition;
            this.actionInvoker = actionInvoker;
            this.stateFactory = stateFactory;
        }

        public Result Evaluate(HttpResponseMessage previousResponse, ApplicationContext context)
        {
            if (condition.IsApplicable(previousResponse, context))
            {
                var newResponse = actionInvoker.Invoke(previousResponse, context);
                return new Result(true, stateFactory.Create(newResponse, context));
            }

            return Result.Unsuccessful;
        }
    }
}
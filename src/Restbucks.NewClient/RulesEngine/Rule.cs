using System.Net.Http;
using Restbucks.RestToolkit.Utils;

namespace Restbucks.NewClient.RulesEngine
{
    public class Rule : IRule
    {
        private readonly ICondition condition;
        private IActionInvoker actionInvoker;
        private readonly CreateActionInvoker createActionInvoker;
        private readonly IStateFactory stateFactory;

        public Rule(ICondition condition, CreateActionInvoker createActionInvoker, IStateFactory stateFactory)
        {
            Check.IsNotNull(condition, "condition");
            Check.IsNotNull(createActionInvoker, "createActionInvoker");
            Check.IsNotNull(stateFactory, "stateFactory");

            this.condition = condition;
            this.createActionInvoker = createActionInvoker;
            this.stateFactory = stateFactory;
        }

        public Rule(ICondition condition, IActionInvoker actionInvoker, IStateFactory stateFactory)
        {
            Check.IsNotNull(condition, "condition");
            Check.IsNotNull(actionInvoker, "actionInvoker");
            Check.IsNotNull(stateFactory, "stateFactory");

            this.condition = condition;
            this.actionInvoker = actionInvoker;
            this.stateFactory = stateFactory;
        }

        public Result Evaluate(HttpResponseMessage previousResponse, ApplicationContext context, Actions actions)
        {
            if (condition.IsApplicable(previousResponse, context))
            {
                if (actionInvoker == null)
                {
                    actionInvoker = createActionInvoker(actions);
                }
                var newResponse = actionInvoker.Invoke(previousResponse, context);
                return new Result(true, stateFactory.Create(newResponse, context, actions));
            }

            return Result.Unsuccessful;
        }
    }
}
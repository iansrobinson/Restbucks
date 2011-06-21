using System.Net.Http;
using Restbucks.RestToolkit.Utils;

namespace Restbucks.RestToolkit.RulesEngine
{
    public class Rule : IRule
    {
        private readonly ICondition condition;
        private readonly IActionInvoker actionInvoker;
        private readonly CreateStateDelegate createState;

        public Rule(ICondition condition, IActionInvoker actionInvoker, CreateStateDelegate createState)
        {
            Check.IsNotNull(condition, "condition");
            Check.IsNotNull(actionInvoker, "actionInvoker");
            Check.IsNotNull(createState, "createState");
            
            this.condition = condition;
            this.actionInvoker = actionInvoker;
            this.createState = createState;
        }

        public Result Evaluate(HttpResponseMessage previousResponse, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities)
        {
            if (condition.IsApplicable(previousResponse, stateVariables))
            {
                var newResponse = actionInvoker.Invoke(previousResponse, stateVariables, clientCapabilities);
                return new Result(true, createState(newResponse, stateVariables, clientCapabilities));
            }

            return Result.Unsuccessful;
        }
    }
}
using System.Net.Http;
using Restbucks.RestToolkit.Utils;

namespace Restbucks.NewClient.RulesEngine
{
    public class Rule : IRule
    {
        private readonly ICondition condition;
        private readonly IAction action;
        private readonly IStateFactory stateFactory;

        public Rule(ICondition condition, IAction action, IStateFactory stateFactory)
        {
            Check.IsNotNull(condition, "condition");
            Check.IsNotNull(action, "action");
            Check.IsNotNull(stateFactory, "stateFactory");

            this.condition = condition;
            this.action = action;
            this.stateFactory = stateFactory;
        }

        public Result Evaluate(HttpResponseMessage previousResponse)
        {
            if (condition.IsApplicable(previousResponse))
            {
                var newResponse = action.Execute();
                return new Result(true, stateFactory.Create(newResponse));
            }

            return Result.Unsuccessful;
        }
    }
}
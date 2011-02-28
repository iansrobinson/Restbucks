using System.Net.Http;
using Restbucks.RestToolkit.Utils;

namespace Restbucks.NewClient
{
    public class Rule
    {
        private readonly ICondition condition;
        private readonly IAction action;

        public Rule(ICondition condition, IAction action)
        {
            Check.IsNotNull(condition, "condition");
            Check.IsNotNull(action, "action");
            
            this.condition = condition;
            this.action = action;
        }

        public Result Evaluate(HttpResponseMessage response)
        {
            if (condition.IsApplicable(response))
            {
                return new Result(true, action.Execute());
            }

            return Result.Unsuccessful;
        }
    }
}
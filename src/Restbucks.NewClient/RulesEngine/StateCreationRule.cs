using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public class StateCreationRule : IRule
    {
        private readonly ICondition condition;
        private readonly IStateFactory stateFactory;

        public StateCreationRule(ICondition condition, IStateFactory stateFactory)
        {
            this.condition = condition;
            this.stateFactory = stateFactory;
        }

        public Result Evaluate(HttpResponseMessage newResponse, ApplicationContext context, Actions actions)
        {
            return condition.IsApplicable(newResponse, context) ? new Result(true, stateFactory.Create(newResponse, context, actions)) : Result.Unsuccessful;
        }
    }
}
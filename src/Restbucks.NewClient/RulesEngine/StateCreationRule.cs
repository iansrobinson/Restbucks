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

        public Result Evaluate(HttpResponseMessage newResponse, ApplicationStateVariables stateVariables)
        {
            return condition.IsApplicable(newResponse, stateVariables) ? new Result(true, stateFactory.Create(newResponse, stateVariables)) : Result.Unsuccessful;
        }
    }
}
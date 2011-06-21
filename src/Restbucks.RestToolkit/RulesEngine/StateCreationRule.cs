using System.Net.Http;

namespace Restbucks.RestToolkit.RulesEngine
{
    public class StateCreationRule : IRule
    {
        private readonly ICondition condition;
        private readonly CreateStateDelegate createState;

        public StateCreationRule(ICondition condition, CreateStateDelegate createState)
        {
            this.condition = condition;
            this.createState = createState;
        }

        public Result Evaluate(HttpResponseMessage newResponse, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities)
        {
            return condition.IsApplicable(newResponse, stateVariables) ? new Result(true, createState(newResponse, stateVariables, clientCapabilities)) : Result.Unsuccessful;
        }
    }
}
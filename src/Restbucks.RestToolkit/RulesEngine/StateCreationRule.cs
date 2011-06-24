using System.Net.Http;

namespace Restbucks.RestToolkit.RulesEngine
{
    public class StateCreationRule : IRule
    {
        private readonly ICondition condition;
        private readonly ICreateNextState createNextState;

        public StateCreationRule(ICondition condition, ICreateNextState createNextState)
        {
            this.condition = condition;
            this.createNextState = createNextState;
        }

        public Result Evaluate(HttpResponseMessage newResponse, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities)
        {
            return condition.IsApplicable(newResponse, stateVariables) 
                ? new Result(true, createNextState.Execute(newResponse, stateVariables, clientCapabilities)) 
                : Result.Unsuccessful;
        }
    }
}
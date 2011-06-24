using System.Net.Http;
using Restbucks.RestToolkit.Utils;

namespace Restbucks.RestToolkit.RulesEngine
{
    public class Rule : IRule
    {
        private readonly ICondition condition;
        private readonly IRequestAction requestAction;
        private readonly ICreateNextState createNextState;

        public Rule(ICondition condition, IRequestAction requestAction, ICreateNextState createNextState)
        {
            Check.IsNotNull(condition, "condition");
            Check.IsNotNull(requestAction, "requestAction");
            Check.IsNotNull(createNextState, "createNextState");
            
            this.condition = condition;
            this.requestAction = requestAction;
            this.createNextState = createNextState;
        }

        public Result Evaluate(HttpResponseMessage previousResponse, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities)
        {
            if (condition.IsApplicable(previousResponse, stateVariables))
            {
                var newResponse = requestAction.Execute(previousResponse, stateVariables, clientCapabilities);
                return new Result(true, createNextState.Execute(newResponse, stateVariables, clientCapabilities));
            }

            return Result.Unsuccessful;
        }
    }
}
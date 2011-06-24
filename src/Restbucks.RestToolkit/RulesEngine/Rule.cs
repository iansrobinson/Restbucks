using System.Net.Http;
using Restbucks.RestToolkit.Utils;

namespace Restbucks.RestToolkit.RulesEngine
{
    public class Rule : IRule
    {
        private readonly ICondition condition;
        private readonly IGenerateNextRequest generateNextRequest;
        private readonly CreateStateDelegate createState;

        public Rule(ICondition condition, IGenerateNextRequest generateNextRequest, CreateStateDelegate createState)
        {
            Check.IsNotNull(condition, "condition");
            Check.IsNotNull(generateNextRequest, "actionInvoker");
            Check.IsNotNull(createState, "createState");
            
            this.condition = condition;
            this.generateNextRequest = generateNextRequest;
            this.createState = createState;
        }

        public Result Evaluate(HttpResponseMessage previousResponse, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities)
        {
            if (condition.IsApplicable(previousResponse, stateVariables))
            {
                var newResponse = generateNextRequest.Execute(previousResponse, stateVariables, clientCapabilities);
                return new Result(true, createState(newResponse, stateVariables, clientCapabilities));
            }

            return Result.Unsuccessful;
        }
    }
}
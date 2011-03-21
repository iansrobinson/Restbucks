using System.Net.Http;
using Restbucks.RestToolkit.Utils;

namespace Restbucks.NewClient.RulesEngine
{
    public class Rule : IRule
    {
        private readonly IsApplicableToStateInfoDelegate isApplicableTo;
        private readonly IActionInvoker actionInvoker;
        private readonly IStateFactory stateFactory;

        public Rule(IsApplicableToStateInfoDelegate isApplicableTo, IActionInvoker actionInvoker, IStateFactory stateFactory)
        {
            Check.IsNotNull(isApplicableTo, "condition");
            Check.IsNotNull(actionInvoker, "actionInvoker");
            Check.IsNotNull(stateFactory, "stateFactory");

            this.isApplicableTo = isApplicableTo;
            this.actionInvoker = actionInvoker;
            this.stateFactory = stateFactory;
        }

        public Result Evaluate(HttpResponseMessage previousResponse, ApplicationStateVariables stateVariables)
        {
            if (isApplicableTo(previousResponse, stateVariables))
            {
                var newResponse = actionInvoker.Invoke(previousResponse, stateVariables);
                return new Result(true, stateFactory.Create(newResponse, stateVariables));
            }

            return Result.Unsuccessful;
        }
    }
}
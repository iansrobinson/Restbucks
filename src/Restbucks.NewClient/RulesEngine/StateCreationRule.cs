using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public class StateCreationRule : IRule
    {
        private readonly IsApplicableToStateInfoDelegate isApplicableTo;
        private readonly IStateFactory stateFactory;

        public StateCreationRule(IsApplicableToStateInfoDelegate isApplicableTo, IStateFactory stateFactory)
        {
            this.isApplicableTo = isApplicableTo;
            this.stateFactory = stateFactory;
        }

        public Result Evaluate(HttpResponseMessage newResponse, ApplicationStateVariables stateVariables)
        {
            return isApplicableTo(newResponse, stateVariables) ? new Result(true, stateFactory.Create(newResponse, stateVariables)) : Result.Unsuccessful;
        }
    }
}
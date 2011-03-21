using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public interface IRule
    {
        Result Evaluate(HttpResponseMessage response, ApplicationStateVariables stateVariables);
    }
}
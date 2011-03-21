using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public interface ICondition
    {
        bool IsApplicable(HttpResponseMessage response, ApplicationStateVariables stateVariables);
    }
}
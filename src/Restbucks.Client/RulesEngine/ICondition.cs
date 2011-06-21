using System.Net.Http;

namespace Restbucks.Client.RulesEngine
{
    public interface ICondition
    {
        bool IsApplicable(HttpResponseMessage response, ApplicationStateVariables stateVariables);
    }
}
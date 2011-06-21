using System.Net.Http;

namespace Restbucks.RestToolkit.RulesEngine
{
    public interface ICondition
    {
        bool IsApplicable(HttpResponseMessage response, ApplicationStateVariables stateVariables);
    }
}
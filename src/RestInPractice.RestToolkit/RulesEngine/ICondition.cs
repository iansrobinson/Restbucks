using System.Net.Http;

namespace RestInPractice.RestToolkit.RulesEngine
{
    public interface ICondition
    {
        bool IsApplicable(HttpResponseMessage response, ApplicationStateVariables stateVariables);
    }
}
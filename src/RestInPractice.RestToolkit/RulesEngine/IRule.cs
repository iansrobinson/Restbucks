using System.Net.Http;

namespace RestInPractice.RestToolkit.RulesEngine
{
    public interface IRule
    {
        Result Evaluate(HttpResponseMessage response, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities);
    }
}
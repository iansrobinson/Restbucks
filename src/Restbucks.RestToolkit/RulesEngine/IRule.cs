using System.Net.Http;

namespace Restbucks.RestToolkit.RulesEngine
{
    public interface IRule
    {
        Result Evaluate(HttpResponseMessage response, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities);
    }
}
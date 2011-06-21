using System.Net.Http;

namespace Restbucks.Client.RulesEngine
{
    public interface IRule
    {
        Result Evaluate(HttpResponseMessage response, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities);
    }
}
using System.Net.Http;

namespace Restbucks.RestToolkit.RulesEngine
{
    public interface IActionInvoker
    {
        HttpResponseMessage Invoke(HttpResponseMessage previousResponse, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities);
    }
}
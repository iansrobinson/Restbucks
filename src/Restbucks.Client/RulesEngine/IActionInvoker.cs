using System.Net.Http;

namespace Restbucks.Client.RulesEngine
{
    public interface IActionInvoker
    {
        HttpResponseMessage Invoke(HttpResponseMessage previousResponse, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities);
    }
}
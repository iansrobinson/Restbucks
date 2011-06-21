using System.Net.Http;

namespace Restbucks.Client.RulesEngine
{
    public interface IAction
    {
        HttpResponseMessage Execute(HttpResponseMessage previousResponse, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities);
    }
}
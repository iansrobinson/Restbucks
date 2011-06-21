using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public interface IAction
    {
        HttpResponseMessage Execute(HttpResponseMessage previousResponse, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities);
    }
}
using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public interface IAction
    {
        HttpResponseMessage Execute(HttpResponseMessage response, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities);
    }
}
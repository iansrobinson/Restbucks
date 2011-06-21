using System.Net.Http;

namespace Restbucks.RestToolkit.RulesEngine
{
    public interface IAction
    {
        HttpResponseMessage Execute(HttpResponseMessage previousResponse, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities);
    }
}
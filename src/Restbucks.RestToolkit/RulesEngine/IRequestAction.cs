using System.Net.Http;

namespace Restbucks.RestToolkit.RulesEngine
{
    public interface IRequestAction
    {
        HttpResponseMessage Execute(HttpResponseMessage previousResponse, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities);
    }
}
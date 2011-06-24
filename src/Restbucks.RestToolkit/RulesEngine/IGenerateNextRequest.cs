using System.Net.Http;

namespace Restbucks.RestToolkit.RulesEngine
{
    public interface IGenerateNextRequest
    {
        HttpResponseMessage Execute(HttpResponseMessage previousResponse, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities);
    }
}
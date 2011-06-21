using System.Net.Http;

namespace Restbucks.RestToolkit.RulesEngine
{
    public interface IFormDataStrategy
    {
        HttpContent CreateFormData(HttpResponseMessage previousResponse, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities);
    }
}
using System.Net.Http;

namespace Restbucks.Client.RulesEngine
{
    public interface IFormDataStrategy
    {
        HttpContent CreateFormData(HttpResponseMessage previousResponse, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities);
    }
}
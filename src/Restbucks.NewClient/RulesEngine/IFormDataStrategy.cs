using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public interface IFormDataStrategy
    {
        HttpContent CreateFormData(HttpResponseMessage response, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities);
    }
}
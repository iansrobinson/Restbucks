using System.Net.Http;

namespace RestInPractice.RestToolkit.RulesEngine
{
    public interface IFormDataStrategy
    {
        HttpContent CreateFormData(HttpResponseMessage previousResponse, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities);
    }
}
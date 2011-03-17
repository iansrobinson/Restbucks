using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public interface IFormDataStrategy
    {
        HttpContent CreateFormData(HttpResponseMessage previousResponse, ApplicationContext context, IClientCapabilities clientCapabilities);
    }
}
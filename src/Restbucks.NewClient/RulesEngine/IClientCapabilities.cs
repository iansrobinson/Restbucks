using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public interface IClientCapabilities
    {
        HttpClient GetHttpClient();
        IContentFormatter GetContentFormatter(MediaTypeHeaderValue contentType);
    }
}
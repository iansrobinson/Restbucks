using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.ApplicationServer.Http;

namespace Restbucks.RestToolkit.RulesEngine
{
    public interface IClientCapabilities
    {
        HttpClient GetHttpClient();
        MediaTypeFormatter GetMediaTypeFormatter(MediaTypeHeaderValue contentType);
    }
}
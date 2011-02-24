using System.Net.Http;

namespace Restbucks.Client.RulesEngine
{
    public interface IResponseHandler
    {
        Result<HttpResponseMessage> Handle(HttpResponseMessage response, ApplicationContext context, IHttpClientProvider clientProvider);
    }
}
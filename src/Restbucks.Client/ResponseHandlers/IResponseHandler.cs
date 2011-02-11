using System.Net.Http;

namespace Restbucks.Client.ResponseHandlers
{
    public interface IResponseHandler
    {
        ActionResult Handle(HttpResponseMessage response, ApplicationContext context, IHttpClientProvider clientProvider);
    }
}
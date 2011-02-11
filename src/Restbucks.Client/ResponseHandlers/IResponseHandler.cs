using System.Net.Http;

namespace Restbucks.Client.ResponseHandlers
{
    public interface IResponseHandler
    {
        HandlerResult Handle(HttpResponseMessage response, ApplicationContext context);
    }
}
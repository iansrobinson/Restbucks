using System.Net.Http;
using Restbucks.Client.RulesEngine;

namespace Restbucks.Client.ResponseHandlers
{
    public interface IResponseHandler
    {
        Result<HttpResponseMessage> Handle(HttpResponseMessage response, ApplicationContext context, IHttpClientProvider clientProvider);
    }
}
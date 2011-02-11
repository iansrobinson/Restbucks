using System;
using System.Net.Http;

namespace Restbucks.Client.ResponseHandlers
{
    public class InitializedResponseHandler : IResponseHandler
    {
        public ActionResult Handle(HttpResponseMessage response, ApplicationContext context, IHttpClientProvider clientProvider)
        {
            var entryPointUri = context.Get<Uri>(ApplicationContextKeys.EntryPointUri);

            using (var client = clientProvider.CreateClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, entryPointUri);
                var newResponse = client.Send(request);

                return new ActionResult(true, newResponse);
            }
        }
    }
}
using System;
using System.Net.Http;
using Restbucks.MediaType;

namespace Restbucks.Client.Actions
{
    public class RequestEntryPoint : IAction<Shop>
    {
        private readonly IHttpClientProvider clientProvider;
        private readonly Uri entryPointUri;

        public RequestEntryPoint(IHttpClientProvider clientProvider, Uri entryPointUri)
        {
            this.clientProvider = clientProvider;
            this.entryPointUri = entryPointUri;
        }

        public ActionResult<Shop> Execute()
        {
            var client = clientProvider.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, entryPointUri);
            var response = client.Send(request);

            return new ActionResult<Shop>(true, response);
        }
    }
}
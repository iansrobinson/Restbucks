using System;
using System.Net.Http;
using Restbucks.MediaType;

namespace Restbucks.Client.Actions
{
    public class Initialize
    {
        private readonly IHttpClientProvider clientProvider;
        private readonly Uri entryPointUri;

        public Initialize(IHttpClientProvider clientProvider, Uri entryPointUri)
        {
            this.clientProvider = clientProvider;
            this.entryPointUri = entryPointUri;
        }

        public ActionResult GetEntryPoint()
        {
            using (var client = clientProvider.CreateClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, entryPointUri);
                var response = client.Send(request);

                return new ActionResult(true, response);
            } 
        }
    }
}
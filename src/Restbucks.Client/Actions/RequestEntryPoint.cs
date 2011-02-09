using System;
using Restbucks.MediaType;

namespace Restbucks.Client.Actions
{
    public class RequestEntryPoint : IAction<Shop>
    {
        private readonly IUserAgent userAgent;
        private readonly Uri entryPointUri;

        public RequestEntryPoint(IUserAgent userAgent, Uri entryPointUri)
        {
            this.userAgent = userAgent;
            this.entryPointUri = entryPointUri;
        }

        public ActionResult<Shop> Execute()
        {
            var response = userAgent.Invoke<Shop>(entryPointUri, null);
            return new ActionResult<Shop>(true, response);
        }
    }
}
using System;
using System.Net.Http;
using System.Reflection;
using log4net;

namespace Restbucks.Client.ResponseHandlers
{
    public class RequestForQuoteFormResponseHandler : IResponseHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IHttpClientProvider clientProvider;

        public RequestForQuoteFormResponseHandler(IHttpClientProvider clientProvider)
        {
            this.clientProvider = clientProvider;
        }

        public HandlerResult Handle(HttpResponseMessage response, ApplicationContext context)
        {
            //If form exists
            //use form to select from context
            //submit using Content-Type and method

            return null;
        }
    }
}
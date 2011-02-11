using System.Net.Http;

namespace Restbucks.Client.ResponseHandlers
{
    public class HandlerResult
    {
        private readonly bool isSuccessful;
        private readonly HttpResponseMessage response;

        public HandlerResult(bool isSuccessful, HttpResponseMessage response)
        {
            this.isSuccessful = isSuccessful;
            this.response = response;
        }

        public bool IsSuccessful
        {
            get { return isSuccessful; }
        }

        public HttpResponseMessage Response
        {
            get { return response; }
        }
    }
}
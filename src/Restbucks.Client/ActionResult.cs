using System.Net.Http;

namespace Restbucks.Client
{
    public class ActionResult<T> where T : class
    {
        private readonly bool isSuccessful;
        private readonly HttpResponseMessage response;

        public ActionResult(bool isSuccessful, HttpResponseMessage response)
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
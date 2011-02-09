using Restbucks.RestToolkit.Http;

namespace Restbucks.Client
{
    public class ActionResult<T> where T : class
    {
        private readonly bool isSuccessful;
        private readonly Response<T> response;

        public ActionResult(bool isSuccessful, Response<T> response)
        {
            this.isSuccessful = isSuccessful;
            this.response = response;
        }

        public bool IsSuccessful
        {
            get { return isSuccessful; }
        }

        public Response<T> Response
        {
            get { return response; }
        }
    }
}
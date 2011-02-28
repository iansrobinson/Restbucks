using System.Net.Http;

namespace Restbucks.NewClient
{
    public class Result
    {
        public static readonly Result Unsuccessful = new Result(false, null);
        
        private readonly bool isSuccessful;
        private readonly HttpResponseMessage response;

        public Result(bool isSuccessful, HttpResponseMessage response)
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
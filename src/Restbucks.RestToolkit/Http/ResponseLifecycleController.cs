using System;

namespace Restbucks.RestToolkit.Http
{
    public class ResponseLifecycleController<T> where T : class
    {
        private readonly Uri uri;
        private bool isPrefetched;
        private Response<T> previousResponse;
        
        public ResponseLifecycleController(Uri uri)
        {
            this.uri = uri;
            isPrefetched = false;
        }

        public Response<T> GetResponse(Func<Uri, Response<T>, Response<T>> client)
        {
            var response = isPrefetched ? previousResponse : client(uri, previousResponse);

            previousResponse = response;
            isPrefetched = false;

            return response;
        }

        public void PrefetchResponse(Func<Uri, Response<T>, Response<T>> client)
        {
            previousResponse = client(uri, previousResponse);
            isPrefetched = true;
        }
    }
}
using System;

namespace Restbucks.RestToolkit
{
    public class ResponseLifecycleController<T> where T : class
    {
        private readonly Uri uri;
        private Response<T> previousResponse;
        private Response<T> prefetchedResponse;

        public ResponseLifecycleController(Uri uri)
        {
            this.uri = uri;
        }

        public Response<T> GetResponse(Func<Uri, Response<T>, Response<T>> client)
        {
            if (prefetchedResponse != null)
            {
                return prefetchedResponse;
            }

            var response = client(uri, previousResponse);
            previousResponse = response;

            return response;
        }

        public void PrefetchResponse(Func<Uri, Response<T>, Response<T>> client)
        {
            prefetchedResponse = client(uri, previousResponse);
        }
    }
}
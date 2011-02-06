using System;
using Restbucks.RestToolkit.Utils;

namespace Restbucks.RestToolkit.Http
{
    public class ResponseAccessor<T> : IResponseAccessor<T> where T : class
    {
        public static IResponseAccessor<T> Create(Uri uri)
        {
            if (uri != null && uri.IsAbsoluteUri)
            {
                return new ResponseAccessor<T>(uri);
            }
            return new InaccessibleResponseAccessor(uri);
        }
        
        private readonly Uri uri;
        private bool isPrefetched;
        private Response<T> lastResponse;

        private ResponseAccessor(Uri uri)
        {
            this.uri = uri;
            isPrefetched = false;
        }

        public Response<T> GetResponse(Func<Uri, Response<T>, Response<T>> client)
        {
            lastResponse = isPrefetched ? lastResponse : client(uri, lastResponse);
            isPrefetched = false;
            return lastResponse;
        }

        public void PrefetchResponse(Func<Uri, Response<T>, Response<T>> client)
        {
            lastResponse = client(uri, lastResponse);
            isPrefetched = true;
        }

        public bool IsDereferenceable
        {
            get { return true; }
        }

        private class InaccessibleResponseAccessor : IResponseAccessor<T>
        {
            private readonly Uri uri;

            public InaccessibleResponseAccessor(Uri uri)
            {
                this.uri = uri;
            }

            public Response<T> GetResponse(Func<Uri, Response<T>, Response<T>> client)
            {
                throw new InvalidOperationException(string.Format("Cannot access URI. URI must be an absolute URI. Uri: [{0}].", uri));
            }

            public void PrefetchResponse(Func<Uri, Response<T>, Response<T>> client)
            {
                throw new InvalidOperationException(string.Format("Cannot access URI. URI must be an absolute URI. Uri: [{0}].", uri));
            }

            public bool IsDereferenceable
            {
                get { return false; }
            }
        }
    }
}
using System.Collections.Generic;

namespace Restbucks.RestToolkit.Http
{
    public class Response<T> where T : class
    {
        private readonly int statusCode;
        private readonly Headers headers;
        private readonly T entityBody;

        public Response(int statusCode, IDictionary<string, IEnumerable<string>> headers, T entityBody)
        {
            this.statusCode = statusCode;
            this.headers = new Headers(headers);
            this.entityBody = entityBody;
        }

        public int StatusCode
        {
            get { return statusCode; }
        }

        public  Headers Headers
        {
            get { return headers; }
        }

        public T EntityBody
        {
            get { return entityBody; }
        }
    }
}
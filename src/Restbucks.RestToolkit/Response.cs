using System.Collections.Generic;

namespace Restbucks.RestToolkit
{
    public class Response<T> where T : class
    {
        private readonly int statusCode;
        private readonly IDictionary<string, IEnumerable<string>> headers;
        private readonly T entityBody;

        public Response(int statusCode, IDictionary<string, IEnumerable<string>> headers, T entityBody)
        {
            this.statusCode = statusCode;
            this.headers = headers;
            this.entityBody = entityBody;
        }

        public int StatusCode
        {
            get { return statusCode; }
        }

        public IDictionary<string, IEnumerable<string>> Headers
        {
            get { return headers; }
        }

        public T EntityBody
        {
            get { return entityBody; }
        }
    }
}
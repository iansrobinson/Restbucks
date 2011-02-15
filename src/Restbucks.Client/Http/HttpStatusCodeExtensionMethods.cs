using System.Net;

namespace Restbucks.Client.Http
{
    public static class HttpStatusCodeExtensionMethods
    {
        public static bool Is2XX(this HttpStatusCode statusCode)
        {
            return IsOfClass(200, (int) statusCode);
        }

        private static bool IsOfClass(int statusCodeClass, int statusCode)
        {
            var diff = statusCode - statusCodeClass;
            return diff >= 0 && diff <= 99;
        }
    }
}
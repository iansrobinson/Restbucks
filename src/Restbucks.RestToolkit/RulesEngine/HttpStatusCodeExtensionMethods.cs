using System.Net;

namespace Restbucks.RestToolkit.RulesEngine
{
    public static class HttpStatusCodeExtensionMethods
    {
        public static bool Is1XX(this HttpStatusCode statusCode)
        {
            return IsOfClass(100, (int) statusCode);
        }

        public static bool Is2XX(this HttpStatusCode statusCode)
        {
            return IsOfClass(200, (int)statusCode);
        }

        public static bool Is3XX(this HttpStatusCode statusCode)
        {
            return IsOfClass(300, (int)statusCode);
        }

        public static bool Is4XX(this HttpStatusCode statusCode)
        {
            return IsOfClass(400, (int)statusCode);
        }

        public static bool Is5XX(this HttpStatusCode statusCode)
        {
            return IsOfClass(500, (int)statusCode);
        }

        private static bool IsOfClass(int statusCodeClass, int statusCode)
        {
            var diff = statusCode - statusCodeClass;
            return diff >= 0 && diff <= 99;
        }
    }
}
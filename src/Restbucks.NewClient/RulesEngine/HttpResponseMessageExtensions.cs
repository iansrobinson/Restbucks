using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public static class HttpResponseMessageExtensions
    {
        public static bool LinkExists(this HttpResponseMessage response, ILinkStrategy strategy)
        {
            return strategy.LinkExists(response);
        }

        public static bool FormExists(this HttpResponseMessage response, IFormStrategy strategy)
        {
            return strategy.FormExists(response);
        }
    }
}
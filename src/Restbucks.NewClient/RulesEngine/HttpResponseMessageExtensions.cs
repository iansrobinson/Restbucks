using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public static class HttpResponseMessageExtensions
    {
        public static bool LinkExists(this HttpResponseMessage response, ILinkStrategy strategy)
        {
            return strategy.LinkExists(response);
        }
    }
}
using System.Net.Http;

namespace RestInPractice.RestToolkit.RulesEngine
{
    public static class HttpResponseMessageExtensions
    {
        public static bool ContainsLink(this HttpResponseMessage response, ILink strategy)
        {
            return strategy.LinkExists(response);
        }

        public static bool ContainsForm(this HttpResponseMessage response, IForm strategy)
        {
            return strategy.FormExists(response);
        }
    }
}
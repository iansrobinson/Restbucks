using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public interface ILinkStrategy
    {
        LinkInfo GetLinkInfo(HttpResponseMessage response);
        bool LinkExists(HttpResponseMessage response);
    }
}
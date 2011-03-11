using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public interface ILinkStrategy
    {
        LinkInfo GetLinkInfo(HttpResponseMessage response);
        bool TryGetLinkInfo(HttpResponseMessage response, out LinkInfo linkInfo);
        bool LinkExists(HttpResponseMessage response);
    }
}
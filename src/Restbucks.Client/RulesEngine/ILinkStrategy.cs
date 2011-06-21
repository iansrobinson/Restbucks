using System.Net.Http;

namespace Restbucks.Client.RulesEngine
{
    public interface ILinkStrategy
    {
        LinkInfo GetLinkInfo(HttpResponseMessage response);
        bool LinkExists(HttpResponseMessage response);
    }
}
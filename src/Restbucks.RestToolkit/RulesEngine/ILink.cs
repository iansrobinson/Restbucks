using System.Net.Http;

namespace Restbucks.RestToolkit.RulesEngine
{
    public interface ILink
    {
        LinkInfo GetLinkInfo(HttpResponseMessage response);
        bool LinkExists(HttpResponseMessage response);
    }
}
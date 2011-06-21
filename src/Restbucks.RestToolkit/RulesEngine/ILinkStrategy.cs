using System.Net.Http;

namespace Restbucks.RestToolkit.RulesEngine
{
    public interface ILinkStrategy
    {
        LinkInfo GetLinkInfo(HttpResponseMessage response);
        bool LinkExists(HttpResponseMessage response);
    }
}
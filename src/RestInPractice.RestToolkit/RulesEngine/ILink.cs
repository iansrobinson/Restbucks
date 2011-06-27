using System.Net.Http;

namespace RestInPractice.RestToolkit.RulesEngine
{
    public interface ILink
    {
        LinkInfo GetLinkInfo(HttpResponseMessage response);
        bool LinkExists(HttpResponseMessage response);
    }
}
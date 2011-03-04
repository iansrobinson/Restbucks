using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public interface ILinkInfoFactory
    {
        LinkInfo CreateLinkInfo(HttpResponseMessage response);
    }
}
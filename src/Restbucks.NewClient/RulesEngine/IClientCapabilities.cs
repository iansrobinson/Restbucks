using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public interface IClientCapabilities
    {
        HttpClient HttpClient { get; }
    }
}
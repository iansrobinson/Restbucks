using System;
using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public interface IConditions
    {
        Func<HttpResponseMessage, bool> FormExists(IFormStrategy formStrategy);
        Func<HttpResponseMessage, bool> LinkExists(ILinkStrategy linkStrategy);
    }
}
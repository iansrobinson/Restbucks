using Restbucks.Client.Hypermedia.Strategies;
using Restbucks.RestToolkit.RulesEngine;

namespace Restbucks.Client.Hypermedia
{
    public static class Forms
    {
        public static readonly IFormStrategy RequestForQuote = RestbucksForm.WithId("request-for-quote");
    }
}
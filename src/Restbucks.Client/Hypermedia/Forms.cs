using Restbucks.Client.Hypermedia.Strategies;
using RestInPractice.RestToolkit.RulesEngine;

namespace Restbucks.Client.Hypermedia
{
    public static class Forms
    {
        public static readonly IForm RequestForQuote = RestbucksForm.WithId("request-for-quote");
    }
}
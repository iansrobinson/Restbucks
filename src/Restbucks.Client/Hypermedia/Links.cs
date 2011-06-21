using System;
using Restbucks.Client.Hypermedia.Strategies;
using Restbucks.RestToolkit.RulesEngine;
using Restbucks.MediaType;

namespace Restbucks.Client.Hypermedia
{
    public static class Links
    {
        public static readonly ILinkStrategy Rfq = RestbucksLink.WithRel(new UriLinkRelation(new Uri("http://relations.restbucks.com/rfq")));
    }
}
using System;
using Restbucks.MediaType;
using Restbucks.NewClient.RulesEngine;

namespace Restbucks.NewClient
{
    public static class Links
    {
        public static readonly ILinkStrategy Rfq = RestbucksLink.WithRel(new UriLinkRelation(new Uri("http://relations.restbucks.com/rfq")));
    }
}
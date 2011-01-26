using System;
using Restbucks.MediaType;

namespace Restbucks.Quoting.Service
{
    public static class LinkRelations
    {
        private static readonly Uri RestbucksLinkRelationsNamespace = new Uri("http://relations.restbucks.com/");
        
        public static readonly LinkRelation Rfq = new CompactUriLinkRelation("rb", RestbucksLinkRelationsNamespace, "rfq");
        public static readonly LinkRelation OrderForm = new CompactUriLinkRelation("rb", RestbucksLinkRelationsNamespace, "order-form");

        public static readonly LinkRelation Self = new StringLinkRelation("self");
        public static readonly LinkRelation Prefetch = new StringLinkRelation("prefetch");
    }
}
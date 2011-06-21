﻿using System;
using Restbucks.Client.RulesEngine;
using Restbucks.MediaType;

namespace Restbucks.Client
{
    public static class Links
    {
        public static readonly ILinkStrategy Rfq = RestbucksLink.WithRel(new UriLinkRelation(new Uri("http://relations.restbucks.com/rfq")));
    }
}
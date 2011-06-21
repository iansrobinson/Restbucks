using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Restbucks.Client.RulesEngine;

namespace Restbucks.Client
{
    public static class Forms
    {
        public static readonly IFormStrategy RequestForQuote = RestbucksForm.WithId("request-for-quote");
    }
}

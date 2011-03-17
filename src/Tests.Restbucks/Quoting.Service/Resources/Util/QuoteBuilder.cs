﻿using Restbucks.Quoting;
using Restbucks.Quoting.Service.Resources;
using Tests.Restbucks.Quoting.Service.Resources.Util;

namespace Tests.Restbucks.Quoting.Service.Resources.helpers
{
    public class QuoteBuilder
    {
        private IQuotationEngine quotationEngine;

        public QuoteBuilder()
        {
            quotationEngine = DummyQuotationEngine.Instance;
        }

        public QuoteBuilder WithQuotationEngine(IQuotationEngine value)
        {
            quotationEngine = value;
            return this;
        }

        public Quote Build()
        {
            return new Quote(DefaultUriFactory.Instance, quotationEngine);
        }
    }
}
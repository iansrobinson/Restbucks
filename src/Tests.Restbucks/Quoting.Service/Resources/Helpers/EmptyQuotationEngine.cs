using System;
using System.Collections.Generic;
using Restbucks.Quoting;

namespace Tests.Restbucks.Quoting.Service.Resources.Helpers
{
    public class EmptyQuotationEngine : IQuotationEngine
    {
        public static readonly IQuotationEngine Instance = new EmptyQuotationEngine();

        private EmptyQuotationEngine()
        {
        }

        public Quotation CreateQuote(QuotationRequest request)
        {
            throw new NotImplementedException();
        }

        public Quotation GetQuote(Guid id)
        {
            throw new KeyNotFoundException();
        }
    }
}
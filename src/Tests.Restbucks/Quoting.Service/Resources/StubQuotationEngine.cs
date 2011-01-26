using System;
using Restbucks.Quoting;

namespace Tests.Restbucks.Quoting.Service.Resources
{
    public class StubQuotationEngine : IQuotationEngine
    {
        private static readonly Quotation DefaultQuotation = new Quotation(Guid.Empty, DateTime.MinValue, new LineItem[] {});


        public Quotation CreateQuote(QuotationRequest request)
        {
            return DefaultQuotation;
        }

        public Quotation GetQuote(Guid id)
        {
            return DefaultQuotation;
        }
    }
}
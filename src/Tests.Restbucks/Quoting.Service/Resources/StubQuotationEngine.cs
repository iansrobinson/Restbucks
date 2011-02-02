using System;
using Restbucks.Quoting;

namespace Tests.Restbucks.Quoting.Service.Resources
{
    public class StubQuotationEngine : IQuotationEngine
    {
        public static readonly IQuotationEngine Instance = new StubQuotationEngine();

        public static readonly Quotation Quotation = new Quotation(
            Guid.Empty,
            DateTime.Now,
            new[]
                    {
                        new LineItem("item1", new Quantity("g", 250), new Money("GBP", 2.50)),
                        new LineItem("item2", new Quantity("kg", 2), new Money("GBP", 2.00))
                    });

        public static readonly string QuoteId = Quotation.Id.ToString("N");

        private StubQuotationEngine()
        {
        }

        public Quotation CreateQuote(QuotationRequest request)
        {
            return Quotation;
        }

        public Quotation GetQuote(Guid id)
        {
            return Quotation;
        }
    }
}
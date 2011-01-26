using Restbucks.Quoting;
using Restbucks.Quoting.Service.Resources;

namespace Tests.Restbucks.Quoting.Service.Resources
{
    public class QuoteBuilder
    {
        private IQuotationEngine quotationEngine;

        public QuoteBuilder()
        {
            quotationEngine = new StubQuotationEngine();
        }

        public QuoteBuilder WithQuotationEngine(IQuotationEngine value)
        {
            quotationEngine = value;
            return this;
        }

        public Quote Build()
        {
            return new Quote(DefaultUriFactoryCollection.Instance, quotationEngine);
        }
    }
}
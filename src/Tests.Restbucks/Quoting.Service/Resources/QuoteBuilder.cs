using Restbucks.Quoting;
using Restbucks.Quoting.Service.Resources;

namespace Tests.Restbucks.Quoting.Service.Resources
{
    public class QuoteBuilder
    {
        private IQuotationEngine quotationEngine;

        public QuoteBuilder()
        {
            quotationEngine = StubQuotationEngine.Instance;
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
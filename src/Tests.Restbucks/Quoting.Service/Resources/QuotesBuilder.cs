using Restbucks.Quoting;
using Restbucks.Quoting.Service.Resources;

namespace Tests.Restbucks.Quoting.Service.Resources
{
    public class QuotesBuilder
    {
        private IQuotationEngine quotationEngine;

        public QuotesBuilder()
        {
            quotationEngine = StubQuotationEngine.Instance;
        }

        public QuotesBuilder WithQuotationEngine(IQuotationEngine value)
        {
            quotationEngine = value;
            return this;
        }

        public Quotes Build()
        {
            return new Quotes(DefaultUriFactoryCollection.Instance, quotationEngine);
        }
    }
}
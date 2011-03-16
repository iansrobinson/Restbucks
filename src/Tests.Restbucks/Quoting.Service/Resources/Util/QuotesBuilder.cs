using Restbucks.Quoting;
using Restbucks.Quoting.Service.Resources;

namespace Tests.Restbucks.Quoting.Service.Resources.Util
{
    public class QuotesBuilder
    {
        private IQuotationEngine quotationEngine;

        public QuotesBuilder()
        {
            quotationEngine = DummyQuotationEngine.Instance;
        }

        public QuotesBuilder WithQuotationEngine(IQuotationEngine value)
        {
            quotationEngine = value;
            return this;
        }

        public Quotes Build()
        {
            return new Quotes(DefaultUriFactory.Instance, quotationEngine);
        }
    }
}
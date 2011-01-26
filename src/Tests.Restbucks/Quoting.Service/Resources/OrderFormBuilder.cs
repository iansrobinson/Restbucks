using Restbucks.Quoting;
using Restbucks.Quoting.Service.Resources;

namespace Tests.Restbucks.Quoting.Service.Resources
{
    public class OrderFormBuilder
    {
        private IQuotationEngine quotationEngine;

        public OrderFormBuilder()
        {
            quotationEngine = new StubQuotationEngine();
        }

        public OrderFormBuilder WithQuotationEngine(IQuotationEngine value)
        {
            quotationEngine = value;
            return this;
        }

        public OrderForm Build()
        {
            return new OrderForm(DefaultUriFactoryCollection.Instance, quotationEngine);
        }
    }
}
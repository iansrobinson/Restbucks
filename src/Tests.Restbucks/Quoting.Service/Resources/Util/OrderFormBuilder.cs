using Restbucks.Quoting;
using Restbucks.Quoting.Service.Resources;

namespace Tests.Restbucks.Quoting.Service.Resources.Util
{
    public class OrderFormBuilder
    {
        private IQuotationEngine quotationEngine;

        public OrderFormBuilder()
        {
            quotationEngine = DummyQuotationEngine.Instance;
        }

        public OrderFormBuilder WithQuotationEngine(IQuotationEngine value)
        {
            quotationEngine = value;
            return this;
        }

        public OrderForm Build()
        {
            return new OrderForm(DefaultUriFactory.Instance, quotationEngine);
        }
    }
}
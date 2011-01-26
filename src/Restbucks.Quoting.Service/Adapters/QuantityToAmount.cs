using Restbucks.MediaType;

namespace Restbucks.Quoting.Service.Adapters
{
    public class QuantityToAmount
    {
        private readonly Quantity quantity;

        public QuantityToAmount(Quantity quantity)
        {
            this.quantity = quantity;
        }

        public Amount Adapt()
        {
            return new Amount(quantity.Measure, quantity.Value);
        }
    }
}
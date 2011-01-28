using Restbucks.MediaType;

namespace Restbucks.Quoting.Service.Old.Adapters
{
    public class MoneyToCost
    {
        private readonly Money money;

        public MoneyToCost(Money money)
        {
            this.money = money;
        }

        public Cost Adapt()
        {
            return new Cost(money.Currency, money.Value);
        }
    }
}
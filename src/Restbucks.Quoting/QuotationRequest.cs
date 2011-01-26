using System.Collections.Generic;

namespace Restbucks.Quoting
{
    public class QuotationRequest
    {
        private readonly IEnumerable<QuotationRequestItem> items;

        public QuotationRequest(IEnumerable<QuotationRequestItem> items)
        {
            this.items = items;
        }

        public IEnumerable<QuotationRequestItem> Items
        {
            get { return items; }
        }
    }
}
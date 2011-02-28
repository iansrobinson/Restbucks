using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Restbucks.Quoting
{
    public class QuotationRequest
    {
        private readonly ReadOnlyCollection<QuotationRequestItem> items;

        public QuotationRequest(IEnumerable<QuotationRequestItem> items)
        {
            this.items = new List<QuotationRequestItem>(items).AsReadOnly();
        }

        public IEnumerable<QuotationRequestItem> Items
        {
            get { return items; }
        }
    }
}
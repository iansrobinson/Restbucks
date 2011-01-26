using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Restbucks.Quoting.Implementation
{
    public class QuotationEngine : IQuotationEngine
    {
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly IGuidProvider guidProvider;
        private readonly ConcurrentDictionary<Guid, Quotation> quotes;

        public QuotationEngine(IDateTimeProvider dateTimeProvider, IGuidProvider guidProvider)
        {
            this.dateTimeProvider = dateTimeProvider;
            this.guidProvider = guidProvider;
            quotes = new ConcurrentDictionary<Guid, Quotation>();
        }

        public Quotation CreateQuote(QuotationRequest request)
        {
            var quote = new Quotation(
                guidProvider.CreateGuid(),
                dateTimeProvider.GetCurrent(),
                request.Items.Select(i => i.CreateLineItem(new Money("GBP", i.Quantity.Value/100.00)))
                );

            return quotes.GetOrAdd(quote.Id, quote);
        }

        public Quotation GetQuote(Guid id)
        {
            return quotes[id];
        }
    }
}
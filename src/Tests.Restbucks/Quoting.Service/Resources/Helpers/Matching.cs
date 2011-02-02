using System;
using System.Collections.Generic;
using System.Linq;
using Restbucks.MediaType;
using Restbucks.Quoting;

namespace Tests.Restbucks.Quoting.Service.Resources.Helpers
{
    public static class Matching
    {
        public static bool MatchSequences<T1, T2>(IEnumerable<T1> seq1, IEnumerable<T2> seq2, Func<T1, T2, bool> comparer)
        {
            if (!seq1.Count().Equals(seq2.Count()))
            {
                return false;
            }

            return seq1.Select((element, index) => comparer(element, seq2.ElementAt(index))).All(result => result.Equals(true));
        }

        public static bool LineItemsMatchItems(IEnumerable<LineItem> lineItems, IEnumerable<Item> items)
        {
            return MatchSequences(lineItems, items, LineItemMatchesItem);
        }

        public static bool QuoteRequestItemsMatchItems(IEnumerable<QuotationRequestItem> quoteRequestItems, IEnumerable<Item> items)
        {
            return MatchSequences(quoteRequestItems, items, QuoteRequestItemMatchesItem);
        }

        public static bool LineItemMatchesItem(LineItem lineItem, Item item)
        {
            return lineItem.Description.Equals(item.Description)
                   && lineItem.Quantity.Measure.Equals(item.Amount.Measure)
                   && lineItem.Quantity.Value.Equals(item.Amount.Value)
                   && lineItem.Price.Currency.Equals(item.Cost.Currency)
                   && lineItem.Price.Value.Equals(item.Cost.Value);
        }

        public static bool QuoteRequestItemMatchesItem(QuotationRequestItem quotationRequestItem, Item item)
        {
            return quotationRequestItem.Description.Equals(item.Description)
                   && quotationRequestItem.Quantity.Measure.Equals(item.Amount.Measure)
                   && quotationRequestItem.Quantity.Value.Equals(item.Amount.Value);
        }
    }
}
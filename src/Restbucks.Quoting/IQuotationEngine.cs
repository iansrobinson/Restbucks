using System;

namespace Restbucks.Quoting
{
    public interface IQuotationEngine
    {
        Quotation CreateQuote(QuotationRequest request);
        Quotation GetQuote(Guid id);
    }
}
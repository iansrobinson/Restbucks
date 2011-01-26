using System;
using System.Collections.Generic;

namespace Restbucks.Quoting
{
    public class Quotation
    {
        private readonly Guid id;
        private readonly DateTimeOffset createdDateTime;
        private readonly IEnumerable<LineItem> lineItems;

        public Quotation(Guid id, DateTimeOffset createdDateTime, IEnumerable<LineItem> lineItems)
        {
            this.id = id;
            this.createdDateTime = createdDateTime;
            this.lineItems = lineItems;
        }

        public Guid Id
        {
            get { return id; }
        }

        public DateTimeOffset CreatedDateTime
        {
            get { return createdDateTime; }
        }

        public IEnumerable<LineItem> LineItems
        {
            get { return lineItems; }
        }
    }
}
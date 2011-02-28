using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Restbucks.Quoting
{
    public class Quotation
    {
        private readonly Guid id;
        private readonly DateTimeOffset createdDateTime;
        private readonly ReadOnlyCollection<LineItem> lineItems;

        public Quotation(Guid id, DateTimeOffset createdDateTime, IEnumerable<LineItem> lineItems)
        {
            this.id = id;
            this.createdDateTime = createdDateTime;
            this.lineItems = new List<LineItem>(lineItems).AsReadOnly();
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
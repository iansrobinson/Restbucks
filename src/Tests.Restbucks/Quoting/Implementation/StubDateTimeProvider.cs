using System;
using Restbucks.Quoting;

namespace Tests.Restbucks.Quoting.Implementation
{
    public class StubDateTimeProvider : IDateTimeProvider
    {
        private readonly DateTimeOffset value;

        public StubDateTimeProvider(DateTimeOffset value)
        {
            this.value = value;
        }

        public DateTimeOffset GetCurrent()
        {
            return value;
        }
    }
}
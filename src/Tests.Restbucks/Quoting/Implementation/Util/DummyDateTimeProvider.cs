using System;
using Restbucks.Quoting;

namespace Tests.Restbucks.Quoting.Implementation.Util
{
    public class DummyDateTimeProvider : IDateTimeProvider
    {
        private readonly DateTimeOffset value;

        public DummyDateTimeProvider(DateTimeOffset value)
        {
            this.value = value;
        }

        public DateTimeOffset GetCurrent()
        {
            return value;
        }
    }
}
using System;

namespace Restbucks.Quoting.Implementation
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTimeOffset GetCurrent()
        {
            return DateTime.Now;
        }
    }
}
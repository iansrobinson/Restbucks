using System;

namespace Restbucks.Quoting
{
    public interface IDateTimeProvider
    {
        DateTimeOffset GetCurrent();
    }
}
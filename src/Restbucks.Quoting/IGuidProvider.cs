using System;

namespace Restbucks.Quoting
{
    public interface IGuidProvider
    {
        Guid CreateGuid();
    }
}
using System;

namespace Restbucks.Quoting.Implementation
{
    public class GuidProvider : IGuidProvider
    {
        public Guid CreateGuid()
        {
            return Guid.NewGuid();
        }
    }
}
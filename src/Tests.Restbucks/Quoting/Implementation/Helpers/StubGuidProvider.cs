using System;
using Restbucks.Quoting;

namespace Tests.Restbucks.Quoting.Implementation.Helpers
{
    public class StubGuidProvider : IGuidProvider
    {
        private readonly Guid value;

        public StubGuidProvider(Guid value)
        {
            this.value = value;
        }

        public Guid CreateGuid()
        {
            return value;
        }
    }
}
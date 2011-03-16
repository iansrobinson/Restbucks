using System;
using Restbucks.Quoting;

namespace Tests.Restbucks.Quoting.Implementation.Util
{
    public class DummyGuidProvider : IGuidProvider
    {
        private readonly Guid value;

        public DummyGuidProvider(Guid value)
        {
            this.value = value;
        }

        public Guid CreateGuid()
        {
            return value;
        }
    }
}
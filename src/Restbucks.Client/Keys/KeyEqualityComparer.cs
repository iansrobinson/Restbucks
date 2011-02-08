using System;
using System.Collections.Generic;

namespace Restbucks.Client.Keys
{
    public class KeyEqualityComparer : IEqualityComparer<IKey>
    {
        public static readonly KeyEqualityComparer Instance = new KeyEqualityComparer();
        
        private KeyEqualityComparer()
        {
        }

        public bool Equals(IKey x, IKey y)
        {
            return StringComparer.InvariantCultureIgnoreCase.Equals(x.Value, y.Value);
        }

        public int GetHashCode(IKey obj)
        {
            return StringComparer.InvariantCultureIgnoreCase.GetHashCode(obj.Value);
        }
    }
}
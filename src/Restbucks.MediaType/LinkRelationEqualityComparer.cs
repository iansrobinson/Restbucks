using System;
using System.Collections.Generic;

namespace Restbucks.MediaType
{
    public class LinkRelationEqualityComparer : IEqualityComparer<LinkRelation>
    {
        public static readonly LinkRelationEqualityComparer Instance = new LinkRelationEqualityComparer();
        
        private LinkRelationEqualityComparer()
        {
        }

        public bool Equals(LinkRelation x, LinkRelation y)
        {
            return StringComparer.InvariantCultureIgnoreCase.Equals(x.Value, y.Value);
        }

        public int GetHashCode(LinkRelation obj)
        {
            return StringComparer.InvariantCultureIgnoreCase.GetHashCode(obj.Value);
        }
    }
}
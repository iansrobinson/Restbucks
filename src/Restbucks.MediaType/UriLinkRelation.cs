using System;

namespace Restbucks.MediaType
{
    public class UriLinkRelation : LinkRelation
    {
        private readonly string value;

        public UriLinkRelation(Uri value)
        {
            this.value = value.OriginalString;
        }

        public override string Value
        {
            get { return value; }
        }

        public override string DisplayValue
        {
            get { return value; }
        }
    }
}
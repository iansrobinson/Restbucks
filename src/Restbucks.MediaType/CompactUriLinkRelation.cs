﻿using System;

namespace Restbucks.MediaType
{
    public class CompactUriLinkRelation : LinkRelation
    {
        private readonly string prefix;
        private readonly Uri uri;
        private readonly string value;
        private readonly string displayValue;

        public CompactUriLinkRelation(string prefix, Uri uri, string reference)
        {
            this.prefix = prefix;
            this.uri = uri;

            value = string.Format("{0}{1}", uri.AbsoluteUri, reference);
            displayValue = string.Format("{0}:{1}", prefix, reference);
        }

        public string Prefix
        {
            get { return prefix; }
        }

        public Uri Uri
        {
            get { return uri; }
        }

        public override string Value
        {
            get { return value; }
        }

        public override string DisplayValue
        {
            get { return displayValue; }
        }
    }
}
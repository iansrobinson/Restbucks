using System;
using System.Collections.Generic;

namespace Restbucks.MediaType
{
    public class Link
    {
        private readonly Uri href;
        private readonly string mediaType;
        private readonly IEnumerable<LinkRelation> rels;

        public Link(Uri href, string mediaType, params LinkRelation[] rels)
        {
            this.href = href;
            this.mediaType = mediaType;
            this.rels = rels;
        }

        public IEnumerable<LinkRelation> Rels
        {
            get { return rels; }
        }

        public string MediaType
        {
            get { return mediaType; }
        }

        public Uri Href
        {
            get { return href; }
        }
    }
}
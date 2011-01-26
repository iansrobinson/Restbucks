using System;
using System.Collections.Generic;
using System.Linq;

namespace Restbucks.MediaType
{
    public class Link
    {
        private readonly IEnumerable<LinkRelation> rels;
        private readonly string mediaType;
        private readonly Uri href;

        public Link(string mediaType, Uri href, params LinkRelation[] rels)
        {
            this.rels = rels;
            this.mediaType = mediaType;
            this.href = href;
        }

        public Link(Uri href, params LinkRelation[] rels) : this(null, href, rels)
        {
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
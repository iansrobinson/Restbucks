using System;
using System.Collections.Generic;

namespace Restbucks.MediaType
{
    public class Link
    {
        private readonly IEnumerable<LinkRelation> rels;
        private readonly string mediaType;
        private readonly Uri href;
        private Shop instance;

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

        public void Prefetch(Func<Uri, Shop> client)
        {
            instance = client(href);
        }

        public Shop Click(Func<Uri, Shop> client)
        {
            return instance ?? client(href);
        }
    }
}
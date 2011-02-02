using System;
using System.Collections.Generic;

namespace Restbucks.MediaType
{
    public class Link
    {
        private readonly IEnumerable<LinkRelation> rels;
        private readonly string mediaType;
        private readonly Uri href;
        private readonly Uri uri;
        private Shop instance;

        private Link(Uri href, Uri uri, string mediaType, params LinkRelation[] rels)
        {
            this.href = href;
            this.uri = uri;
            this.mediaType = mediaType;
            this.rels = rels;           
        }

        public Link(Uri href, string mediaType, params LinkRelation[] rels) : this(href, href.IsAbsoluteUri ? href : null, mediaType, rels)
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
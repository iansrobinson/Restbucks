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
        private readonly Uri absoluteUri;
        private Shop instance;

        public Link(Uri href, string mediaType, params LinkRelation[] rels)
        {
            this.href = href;
            this.mediaType = mediaType;
            this.rels = rels;

            if (href.IsAbsoluteUri)
            {
                absoluteUri = href;
            }
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
            if (absoluteUri == null)
            {
                throw new InvalidOperationException("Unable to determine absolute URI.");
            }
            
            return instance ?? client(href);
        }

        public Link NewLinkWithBaseUri(Uri baseUri)
        {
            if (absoluteUri != null)
            {
                throw new InvalidOperationException("Link is already backed by an absolute URI.");
            }
            
            return new Link(new Uri(baseUri, href), mediaType, rels.ToArray());
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Restbucks.RestToolkit.Http;

namespace Restbucks.MediaType
{
    public class Link
    {
        private readonly IEnumerable<LinkRelation> rels;
        private readonly string mediaType;
        private readonly Href href;
        
        private Link(Href href, string mediaType, params LinkRelation[] rels)
        {
            this.href = href;
            this.mediaType = mediaType;
            this.rels = rels;    
        }

        public Link(Uri href, string mediaType, params LinkRelation[] rels) : this(new Href(href), mediaType, rels)
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
            get { return href.Uri; }
        }

        public bool IsClickable
        {
            get { return href.IsDereferenceable; }
        }

        public void Prefetch(Func<Uri, Response<Shop>, Response<Shop>> client)
        {
            href.ResponseLifecycleController.PrefetchResponse(client);
        }

        public Response<Shop> Click(Func<Uri, Response<Shop>, Response<Shop>> client)
        {
            return href.ResponseLifecycleController.GetResponse(client);
        }

        public Link NewLinkWithBaseUri(Uri baseUri)
        {
            var fullUri = new UriTemplate(href.Uri.ToString()).BindByPosition(baseUri);
            return new Link(href.WithNewAccessUri(fullUri), mediaType, rels.ToArray());
        }
    }
}
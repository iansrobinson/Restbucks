using System;
using System.Collections.Generic;
using System.Linq;
using Restbucks.RestToolkit.Http;

namespace Restbucks.MediaType
{
    public class Link
    {
        private readonly Uri href;
        private readonly IResponseAccessor<Shop> responseAccessor;
        private readonly string mediaType;
        private readonly IEnumerable<LinkRelation> rels;

        private Link(Uri href, IResponseAccessor<Shop> responseAccessor, string mediaType, params LinkRelation[] rels)
        {
            this.href = href;
            this.responseAccessor = responseAccessor;
            this.mediaType = mediaType;
            this.rels = rels;
        }

        public Link(Uri href, string mediaType, params LinkRelation[] rels) : this(href, ResponseAccessor<Shop>.Create(href), mediaType, rels)
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

        public bool IsClickable
        {
            get { return responseAccessor.IsDereferenceable; }
        }

        public void Prefetch(Func<Uri, Response<Shop>, Response<Shop>> client)
        {
            responseAccessor.PrefetchResponse(client);
        }

        public Response<Shop> Click(Func<Uri, Response<Shop>, Response<Shop>> client)
        {
            return responseAccessor.GetResponse(client);
        }

        public Link NewLinkWithBaseUri(Uri baseUri)
        {
            var absoluteUri = new UriTemplate(href.ToString()).BindByPosition(baseUri);
            return new Link(href, ResponseAccessor<Shop>.Create(absoluteUri), mediaType, rels.ToArray());
        }
    }
}
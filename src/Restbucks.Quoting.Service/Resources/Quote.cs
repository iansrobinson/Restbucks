using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.ServiceModel.Web;
using Restbucks.MediaType;
using Restbucks.Quoting.Service.Adapters;
using Restbucks.RestToolkit;

namespace Restbucks.Quoting.Service.Resources
{
    [ServiceContract]
    [UriTemplate("quote", "{id}")]
    public class Quote
    {
        private const string QuoteUriTemplate = "{id}";

        private readonly UriFactory uriFactory;
        private readonly IQuotationEngine quotationEngine;

        public Quote(UriFactory uriFactory, IQuotationEngine quotationEngine)
        {
            this.uriFactory = uriFactory;
            this.quotationEngine = quotationEngine;
        }

        [WebGet(UriTemplate = QuoteUriTemplate)]
        public Shop Get(string id, HttpRequestMessage request, HttpResponseMessage response)
        {
            Quotation quotation;
            try
            {
                quotation = quotationEngine.GetQuote(new Guid(id));
            }
            catch (KeyNotFoundException)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                return null;
            }

            var baseUri = uriFactory.CreateBaseUri<Quote>(request.RequestUri);

            response.StatusCode = HttpStatusCode.OK;
            response.Headers.CacheControl = new CacheControlHeaderValue { Public = true };
            response.Content = new ByteArrayContent(new byte[] { });
            response.Content.Headers.Expires = quotation.CreatedDateTime.AddDays(7.0);

            return new Shop(baseUri, quotation.LineItems.Select(li => new LineItemToItem(li).Adapt()))
                .AddLink(new Link(uriFactory.CreateRelativeUri<Quote>(quotation.Id), RestbucksMediaType.Value, LinkRelations.Self))
                .AddLink(new Link(uriFactory.CreateRelativeUri<OrderForm>(quotation.Id), RestbucksMediaType.Value, LinkRelations.OrderForm));
        }
    }
}
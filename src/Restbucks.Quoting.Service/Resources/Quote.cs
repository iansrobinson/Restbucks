﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.ServiceModel.Web;
using Restbucks.MediaType;
using Restbucks.Quoting.Service.Adapters;

namespace Restbucks.Quoting.Service.Resources
{
    [ServiceContract]
    [UriTemplate("quote", "{id}")]
    public class Quote
    {
        private const string QuoteUriTemplate = "{id}";

        private readonly UriFactoryCollection uriFactories;
        private readonly IQuotationEngine quotationEngine;

        public Quote(UriFactoryCollection uriFactories, IQuotationEngine quotationEngine)
        {
            this.uriFactories = uriFactories;
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

            response.StatusCode = HttpStatusCode.OK;
            response.Headers.CacheControl = new CacheControlHeaderValue { Public = true };
            response.Content = new ByteArrayContent(new byte[] { });
            response.Content.Headers.Expires = quotation.CreatedDateTime.AddDays(7.0);

            return CreateEntityBody(quotation, request.RequestUri);
        }

        private Shop CreateEntityBody(Quotation quotation, Uri requestUri)
        {
            var baseUri = uriFactories.For<Quote>().CreateBaseUri(requestUri);
            var uri = GenerateQuoteUri(baseUri, quotation);

            return new Shop(baseUri, quotation.LineItems.Select(li => new LineItemToItem(li).Adapt()))
                .AddLink(new Link(new Uri(uri.PathAndQuery, UriKind.Relative), "application/restbucks+xml", LinkRelations.Self))
                .AddLink(new Link(uriFactories.For<OrderForm>().CreateRelativeUri(quotation.Id.ToString("N")), "application/restbucks+xml", LinkRelations.OrderForm));
        }

        private Uri GenerateQuoteUri(Uri baseUri, Quotation quotation)
        {
            return uriFactories.For<Quote>().CreateAbsoluteUri(baseUri, quotation.Id.ToString("N"));
        }
    }
}
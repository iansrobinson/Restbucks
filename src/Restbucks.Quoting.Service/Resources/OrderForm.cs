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
    [UriTemplate("order-form", OrderFormUriTemplate)]
    public class OrderForm
    {
        public const string SignedFormPlaceholder = "SIGNED_FORM_PLACEHOLDER";
        
        private static readonly UriFactory OrdersUriFactory = new UriFactory("orders", string.Format("?c=12345&s={0}", SignedFormPlaceholder));
        private const string OrderFormUriTemplate = "{id}";

        private readonly UriFactoryCollection uriFactories;
        private readonly IQuotationEngine quotationEngine;

        public OrderForm(UriFactoryCollection uriFactories, IQuotationEngine quotationEngine)
        {
            this.uriFactories = uriFactories;
            this.quotationEngine = quotationEngine;
        }

        [WebGet(UriTemplate = OrderFormUriTemplate)]
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
            response.Headers.CacheControl = new CacheControlHeaderValue {Public = true};
            response.Content = new ByteArrayContent(new byte[] {});
            response.Content.Headers.Expires = quotation.CreatedDateTime.AddDays(7.0);
            response.Content.Headers.ContentLocation = uriFactories.For<Quote>().CreateAbsoluteUri(request.RequestUri, quotation.Id.ToString("N"));

            return new Shop()
                .AddForm(new Form(
                             OrdersUriFactory.CreateAbsoluteUri(new Uri("http://localhost:8081")),
                             "post",
                             "application/restbucks+xml",
                             new Shop(quotation.LineItems.Select(li => new LineItemToItem(li).Adapt()))
                                 .AddLink(new Link(uriFactories.For<Quote>().CreateRelativeUri(quotation.Id.ToString("N")), LinkRelations.Self))));
        }
    }
}
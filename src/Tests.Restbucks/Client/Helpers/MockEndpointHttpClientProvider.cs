﻿//using System;
//using System.Net.Http;
//using Restbucks.Client;
//using Restbucks.Client.Http;
//
//namespace Tests.Restbucks.Client.Helpers
//{
//    public class MockEndpointHttpClientProvider : IHttpClientProvider
//    {
//        private readonly MockEndpoint endpoint;
//
//        public MockEndpointHttpClientProvider(MockEndpoint endpoint)
//        {
//            this.endpoint = endpoint;
//        }
//
//        public HttpClient CreateClient()
//        {
//            return CreateClient(null);
//        }
//
//        public HttpClient CreateClient(Uri baseUri)
//        {
//            var client = HttpClientProvider.Instance.CreateClient(baseUri);
//            client.Channel = endpoint;
//            return client;
//        }
//    }
//}
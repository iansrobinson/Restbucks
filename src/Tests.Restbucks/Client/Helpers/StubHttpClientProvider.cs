//using System;
//using System.Net.Http;
//using System.Threading;
//using Restbucks.Client;
//using Restbucks.Client.Http;
//
//namespace Tests.Restbucks.Client.Helpers
//{
//    public class StubHttpClientProvider : IHttpClientProvider
//    {
//        private readonly HttpResponseMessage response;
//
//        public StubHttpClientProvider() : this(new HttpResponseMessage())
//        {
//        }
//
//        public StubHttpClientProvider(HttpResponseMessage response)
//        {
//            this.response = response;
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
//            client.Channel = new StubEndpoint(response);
//            return client;
//        }
//
//        private class StubEndpoint : HttpClientChannel
//        {
//            private readonly HttpResponseMessage response;
//
//            public StubEndpoint(HttpResponseMessage response)
//            {
//                this.response = response;
//            }
//
//            protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
//            {
//                return response;
//            }
//        }
//    }
//}
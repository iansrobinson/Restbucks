using Restbucks.Quoting.Service.Resources;

namespace Tests.Restbucks.Quoting.Service.Resources.Helpers
{
    public static class DefaultUriFactory
    {
        public static UriFactory Instance
        {
            get
            {
                var uriFactories = new UriFactory();
                uriFactories.Register<EntryPoint>();
                uriFactories.Register<RequestForQuote>();
                uriFactories.Register<Quotes>();
                uriFactories.Register<Quote>();
                uriFactories.Register<OrderForm>();
                return uriFactories;
            }
        }
    }
}
using Restbucks.Quoting.Service.Resources;

namespace Tests.Restbucks.Quoting.Service.Resources.Helpers
{
    public static class DefaultUriFactoryCollection
    {
        public static UriFactoryCollection Instance
        {
            get
            {
                var uriFactories = new UriFactoryCollection();
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
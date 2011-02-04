using Restbucks.Quoting.Service.Resources;
using Restbucks.RestToolkit;

namespace Tests.Restbucks.Quoting.Service.Resources.Helpers
{
    public static class DefaultUriFactory
    {
        public static UriFactory Instance
        {
            get
            {
                var uriFactory = new UriFactory();
                uriFactory.Register<EntryPoint>();
                uriFactory.Register<RequestForQuote>();
                uriFactory.Register<Quotes>();
                uriFactory.Register<Quote>();
                uriFactory.Register<OrderForm>();
                return uriFactory;
            }
        }
    }
}
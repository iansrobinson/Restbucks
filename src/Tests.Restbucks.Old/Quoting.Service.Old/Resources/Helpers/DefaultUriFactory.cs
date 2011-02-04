using Restbucks.Quoting.Service.Old.Resources;
using Restbucks.RestToolkit;

namespace Tests.Restbucks.Old.Quoting.Service.Old.Resources.Helpers
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
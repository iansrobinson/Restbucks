using Restbucks.Quoting.Service.Old.Resources;

namespace Tests.Restbucks.Old.Quoting.Service.Old.Resources.Helpers
{
    public static class DefaultUriFactory
    {
        public static NewUriFactory Instance
        {
            get
            {
                var uriFactory = new NewUriFactory();
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
using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Web;
using System.Web.Routing;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using log4net;
using log4net.Config;
using Microsoft.ApplicationServer.Http.Description;
using Microsoft.ApplicationServer.Http.Dispatcher;
using Restbucks.MediaType;
using Restbucks.Quoting.Implementation;
using Restbucks.Quoting.Service.Configuration;
using Restbucks.Quoting.Service.MessageHandlers.FormsIntegrity;
using Restbucks.Quoting.Service.Resources;
using Restbucks.RestToolkit.Hypermedia;

namespace Restbucks.Quoting.Service
{
    public class Global : HttpApplication
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IWindsorContainer container;

        public Global()
        {
            container = new WindsorContainer();
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            XmlConfigurator.Configure();

            Log.Debug("Starting Restbucks.Quoting.Service...");

            var uriFactory = new UriFactory();

            container.Register(Component.For(typeof (IQuotationEngine)).ImplementedBy(typeof (QuotationEngine)).LifeStyle.Singleton);
            container.Register(Component.For(typeof (IDateTimeProvider)).ImplementedBy(typeof (DateTimeProvider)).LifeStyle.Singleton);
            container.Register(Component.For(typeof (IGuidProvider)).ImplementedBy(typeof (GuidProvider)).LifeStyle.Singleton);
            container.Register(Component.For(typeof (UriFactory)).Instance(uriFactory).LifeStyle.Singleton);

            var formsIntegrityUtility = new FormsIntegrityUtility(Signature.Instance, OrderForm.SignedFormPlaceholder);
            Action<Collection<HttpOperationHandler>> handlers = c => c.Add(new FormsIntegrityResponseHandler(formsIntegrityUtility));

            var configuration = HttpHostConfiguration.Create()
                .SetResourceFactory(new ResourceFactory(container))
                .AddFormatters(RestbucksMediaTypeFormatter.Instance)
                .AddResponseHandlers(handlers, (endpoint, operation) => operation.DeclaringContract.ContractType.Equals(typeof (OrderForm)));

            new ResourceManager(configuration, container, RouteTable.Routes).RegisterResourcesFor(Assembly.GetExecutingAssembly());
        }

        protected void Application_End(object sender, EventArgs e)
        {
            Log.Debug("Shutting down Restbucks.Quoting.Service...");
            container.Dispose();
        }
    }
}
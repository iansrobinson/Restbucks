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
using Restbucks.Quoting.Service.MessageHandlers.FormsIntegrity;
using Restbucks.Quoting.Service.Resources;
using Restbucks.RestToolkit.Configuration;
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

            var formsIntegrityUtility = new FormsIntegrityUtility(Signature.Instance, OrderForm.SignedFormPlaceholder);
            Action<Collection<HttpOperationHandler>> handlers = c => c.Add(new FormsIntegrityResponseHandler(formsIntegrityUtility));

            var configuration = HttpHostConfiguration.Create()
                .SetResourceFactory(
                    (type, ctx, message) =>
                        {
                            Log.DebugFormat("Getting instance of type [{0}].", type.FullName);
                            return container.GetService(type);
                        },
                    (ctx, service) =>
                        {
                            if (service is IDisposable)
                            {
                                Log.DebugFormat("Calling Dispose() on instance of type [{0}].", service.GetType().FullName);
                                ((IDisposable) service).Dispose();
                            }
                        })
                .AddFormatters(RestbucksMediaTypeFormatter.Instance)
                .AddResponseHandlers(handlers, (endpoint, operation) => operation.DeclaringContract.ContractType.Equals(typeof (OrderForm)));

            var uriFactory = new UriFactory();
            var resources = new ResourceCollection(Assembly.GetExecutingAssembly());
            resources.ForEach(r =>
                                  {
                                      container.Register(Component.For(r.Type).LifeStyle.Transient);
                                      uriFactory.Register(r.Type);
                                      RouteTable.Routes.MapServiceRoute(r.Type, r.UriTemplate.RoutePrefix, configuration);
                                      Log.DebugFormat("Registered resource. Type: [{0}]. Prefix: [{1}]. UriTemplate: [{2}].", r.Type.Name, r.UriTemplate.RoutePrefix, r.UriTemplate.UriTemplateValue);
                                  });

            container.Register(Component.For(typeof (IQuotationEngine)).ImplementedBy(typeof (QuotationEngine)).LifeStyle.Singleton);
            container.Register(Component.For(typeof (IDateTimeProvider)).ImplementedBy(typeof (DateTimeProvider)).LifeStyle.Singleton);
            container.Register(Component.For(typeof (IGuidProvider)).ImplementedBy(typeof (GuidProvider)).LifeStyle.Singleton);
            container.Register(Component.For(typeof (UriFactory)).Instance(uriFactory).LifeStyle.Singleton);
        }

        protected void Application_End(object sender, EventArgs e)
        {
            Log.Debug("Shutting down Restbucks.Quoting.Service...");
            container.Dispose();
        }
    }
}
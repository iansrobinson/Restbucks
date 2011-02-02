using System;
using System.Reflection;
using System.Web;
using System.Web.Routing;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using log4net;
using log4net.Config;
using Microsoft.ServiceModel.Http;
using Restbucks.Quoting.Implementation;
using Restbucks.Quoting.Service.Old.Processors;
using Restbucks.Quoting.Service.Old.Resources;

namespace Restbucks.Quoting.Service.Old
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

            Log.Debug("Starting Restbucks.Quoting.Service.Old ...");

            container.Register(Component.For(typeof (IQuotationEngine)).ImplementedBy(typeof (QuotationEngine)).LifeStyle.Singleton);
            container.Register(Component.For(typeof (IDateTimeProvider)).ImplementedBy(typeof (DateTimeProvider)).LifeStyle.Singleton);
            container.Register(Component.For(typeof (IGuidProvider)).ImplementedBy(typeof (GuidProvider)).LifeStyle.Singleton);
            container.Register(Component.For(typeof (ISignForms)).Instance(new FormsIntegrityUtility(Signature.Instance, OrderForm.SignedFormPlaceholder)).LifeStyle.Singleton);
            container.Register(Component.For(typeof (FormsIntegrityResponseProcessor)).LifeStyle.Singleton);

            container.Register(Component.For(typeof (EntryPoint)).LifeStyle.Transient);
            container.Register(Component.For(typeof (RequestForQuote)).LifeStyle.Transient);
            container.Register(Component.For(typeof (Quotes)).LifeStyle.Transient);
            container.Register(Component.For(typeof (OrderForm)).LifeStyle.Transient);

            var configuration = new Config(container);

            RouteTable.Routes.AddServiceRoute<EntryPoint>(EntryPoint.UriFactory.RoutePrefix, configuration);
            RouteTable.Routes.AddServiceRoute<RequestForQuote>(RequestForQuote.UriFactory.RoutePrefix, configuration);
            RouteTable.Routes.AddServiceRoute<Quotes>(Quotes.QuotesUriFactory.RoutePrefix, configuration);
            RouteTable.Routes.AddServiceRoute<OrderForm>("order-forms", configuration);
        }

        protected void Application_End(object sender, EventArgs e)
        {
            Log.Debug("Shutting down Restbucks.Quoting.Service.Old ...");
            container.Dispose();
        }
    }
}
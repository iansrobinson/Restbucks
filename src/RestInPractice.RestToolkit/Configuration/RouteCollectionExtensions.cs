using System;
using System.ServiceModel.Activation;
using System.Web.Routing;
using Microsoft.ApplicationServer.Http.Activation;
using Microsoft.ApplicationServer.Http.Description;

namespace RestInPractice.RestToolkit.Configuration
{
    public static class RouteCollectionExtensions
    {
        public static void MapServiceRoute(this RouteCollection routes, Type serviceType, string routePrefix, IHttpHostConfigurationBuilder builder = null, params object[] constraints)
        {
            if (routes == null)
            {
                throw new ArgumentNullException("routes");
            }
            var route = new ServiceRoute(routePrefix, new HttpConfigurableServiceHostFactory {Builder = builder}, serviceType);
            routes.Add(route);
        }
    }
}
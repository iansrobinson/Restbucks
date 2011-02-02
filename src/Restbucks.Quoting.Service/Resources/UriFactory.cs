using System;
using System.Collections.Generic;

namespace Restbucks.Quoting.Service.Resources
{
    public class UriFactory
    {
        private readonly IDictionary<Type, UriFactoryWorker> uriFactories;

        public UriFactory()
        {
            uriFactories = new Dictionary<Type, UriFactoryWorker>();
        }

        public void Register<T>() where T : class
        {
            var attributes = typeof(T).GetCustomAttributes(typeof(UriTemplateAttribute), false);
            if (attributes.Length == 0)
            {
                throw new UriTemplateMissingException();
            }
            var uriFactory =  ((UriTemplateAttribute)attributes[0]).UriFactoryWorker;

            uriFactories.Add(typeof(T), uriFactory);
        }

        public UriFactoryWorker For<T>()
        {
            return uriFactories[typeof (T)];
        }
    }
}
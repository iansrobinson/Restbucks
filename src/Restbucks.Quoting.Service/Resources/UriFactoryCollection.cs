using System;
using System.Collections.Generic;

namespace Restbucks.Quoting.Service.Resources
{
    public class UriFactoryCollection
    {
        private readonly IDictionary<Type, UriFactory> uriFactories;

        public UriFactoryCollection()
        {
            uriFactories = new Dictionary<Type, UriFactory>();
        }

        public void Register<T>() where T : class
        {
            var attributes = typeof(T).GetCustomAttributes(typeof(UriTemplateAttribute), false);
            if (attributes.Length == 0)
            {
                throw new UriTemplateMissingException();
            }
            var uriFactory =  ((UriTemplateAttribute)attributes[0]).UriFactory;

            uriFactories.Add(typeof(T), uriFactory);
        }

        public UriFactory For<T>()
        {
            return uriFactories[typeof (T)];
        }
    }
}
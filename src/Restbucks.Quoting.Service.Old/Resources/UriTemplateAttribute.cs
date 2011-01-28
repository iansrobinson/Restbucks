using System;

namespace Restbucks.Quoting.Service.Old.Resources
{
    [AttributeUsage(AttributeTargets.Method)]
    public class UriTemplateAttribute : Attribute
    {
        private readonly string value;

        public UriTemplateAttribute(string value)
        {
            this.value = value;
        }

        public string Value
        {
            get { return value; }
        }
    }
}
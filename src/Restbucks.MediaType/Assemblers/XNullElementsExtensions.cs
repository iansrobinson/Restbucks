using System;
using System.Xml;
using System.Xml.Linq;

namespace Restbucks.MediaType.Assemblers
{
    public static class XNullElementsExtensions
    {
        public static dynamic ApplyNonEmptyValueOrDefault(this XObject target)
        {
            return ApplyNonEmptyValueOrDefault(target, s => s, null);
        }

        public static dynamic ApplyNonEmptyValueOrDefault(this XObject target, Func<string, object> functionToApply)
        {
            return ApplyNonEmptyValueOrDefault(target, functionToApply, null);
        }

        public static dynamic ApplyNonEmptyValueOrDefault(this XObject target, Func<string, object> functionToApply, object defaultValue)
        {
            if (target == null)
            {
                return defaultValue;
            }
            string value = target.NodeType.Equals(XmlNodeType.Element) ? ((XElement) target).Value : ((XAttribute) target).Value;
            if (String.IsNullOrWhiteSpace(value))
            {
                return defaultValue;
            }
            return functionToApply(value);
        }

        public static dynamic ApplyNonNullInstance<T>(this T target, Func<T, object> functionToApply) where T : XObject
        {
            if (target == null)
            {
                return null;
            }
            return functionToApply(target);
        }
    }
}
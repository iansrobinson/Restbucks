using System.Reflection;

namespace Tests.Restbucks.Util
{
    public static class PrivateField
    {
        public static T GetPrivateFieldValue<T>(this object o, string fieldName)
        {
            var fieldInfo = o.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic);
            return (T)fieldInfo.GetValue(o);
        }
    }
}
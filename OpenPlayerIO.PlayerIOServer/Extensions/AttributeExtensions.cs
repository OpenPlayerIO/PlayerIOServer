using System;
using System.Linq;

namespace OpenPlayerIO.PlayerIOServer.Extensions
{
    public static class AttributeExtensions
    {
        public static TValue GetAttributeValue<TAttribute, TValue>(this Type type, Func<TAttribute, TValue> valueSelector) where TAttribute : Attribute
        {
            var attribute = type.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() as TAttribute;
            return (attribute != null) ? valueSelector(attribute) : default(TValue);
        }
    }
}
namespace Omnipaste.ExtensionMethods
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    public static class PropertyExtensions
    {
        public static string GetPropertyName<T, K>(this T obj, Expression<Func<T, K>> property)
        {
            var propertyInfo = (property.Body as MemberExpression).Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException("The lambda expression 'property' should point to a valid Property");
            }
            return propertyInfo.Name;
        }
    }
}

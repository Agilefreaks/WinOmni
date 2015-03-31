namespace Omnipaste.Framework.ExtensionMethods
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    public static class PropertyExtensions
    {
        public static string GetPropertyName<T, TK>(this T obj, Expression<Func<T, TK>> property)
        {
            return GetPropertyName(property);
        }

        public static string GetPropertyName<T, TK>(Expression<Func<T, TK>> property)
        {
            var memberExpression = property.Body as MemberExpression;
            if (memberExpression == null)
            {
                return null;
            }

            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException("The lambda expression 'property' should point to a valid Property");
            }
            return propertyInfo.Name;
        }
    }
}
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;

namespace PubNubClipboard.ExtensionMethods
{
    public static class UriExtensionMethods
    {
        public static NameValueCollection GetQueryStringParameters(this Uri uri)
        {
            NameValueCollection result = null;
            if (uri != null)
            {
                var queryString = uri.Query;
                var nameValueCollection = new NameValueCollection();
                if (queryString.Contains("?"))
                {
                    queryString = queryString.Substring(queryString.IndexOf('?') + 1);
                }

                foreach (var singlePair in Regex.Split(queryString, "&").Select(nameValuePair => Regex.Split(nameValuePair, "=")))
                {
                    nameValueCollection.Add(singlePair[0], singlePair.Length == 2 ? singlePair[1] : string.Empty);
                }

                result = nameValueCollection;
            }

            return result;
        }
    }
}

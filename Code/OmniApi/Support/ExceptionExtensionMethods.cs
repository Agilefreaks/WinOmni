namespace OmniApi.Support
{
    using System;
    using System.Net;
    using Refit;

    public static class ExceptionExtensionMethods
    {
        public static ApiException GetApiException(this Exception exception)
        {
            ApiException result = null;

            var apiException = exception as ApiException;
            if (apiException != null)
            {
                result = apiException;
            }
            else if (exception is AggregateException && exception.InnerException is ApiException)
            {
                result = (ApiException)exception.InnerException;
            }

            return result;
        }

        public static bool HasStatusCode(this ApiException exception, HttpStatusCode statusCode)
        {
            return exception != null && exception.StatusCode == statusCode;
        }

        public static bool IsHttpError(this Exception exception, HttpStatusCode statusCode)
        {
            return GetApiException(exception).HasStatusCode(statusCode);
        }
    }
}

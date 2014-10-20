namespace Omnipaste.ExtensionMethods
{
    using System;
    using BugFreak;
    using NAppUpdate.Framework.Common;

    public static class AsyncResultExtensionMethods
    {
        public static bool CompleteSafely(this IAsyncResult asyncResult)
        {
            if (!asyncResult.IsCompleted) return false;

            var couldHandle = true;

            try
            {
                var updateProcessAsyncResult = asyncResult as UpdateProcessAsyncResult;
                if (updateProcessAsyncResult != null)
                {
                    updateProcessAsyncResult.EndInvoke();
                }
            }
            catch (Exception exception)
            {
                couldHandle = false;
                ReportingService.Instance.BeginReport(exception);
            }

            return couldHandle;
        }
    }
}
using System;
using Common.Logging;

namespace ClipboardWatcher.Core.ExtensionMethods
{
    public static class ObjectExtensionMethods
    {
        public static void Log<T>(this T source, string message)
        {
            LogManager.GetLogger<T>().Info(message);
        }

        public static void Log<T>(this T source, Exception exception)
        {
            LogManager.GetLogger<T>().Error(exception);
        }
    }
}

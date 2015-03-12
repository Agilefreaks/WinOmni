namespace OmniCommon.Helpers
{
    using System;
    using System.Reactive.Disposables;

    public class TimeHelper
    {
        private static DateTime? _utcNow;

        public static DateTime UtcNow
        {
            get
            {
                return _utcNow ?? DateTime.UtcNow;
            }
            set
            {
                _utcNow = value;
            }
        }

        public static void Reset()
        {
            _utcNow = null;
        }

        public static IDisposable Freez()
        {
            UtcNow = DateTime.UtcNow;
            return Disposable.Create(Reset);
        }
    }
}
namespace OmniCommon.Helpers
{
    using System;

    public class TimeHelper
    {
        #region Static Fields

        private static DateTime? _utcNow;

        #endregion

        #region Public Properties

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

        #endregion

        #region Public Methods and Operators

        public static void Reset()
        {
            _utcNow = null;
        }

        #endregion
    }
}
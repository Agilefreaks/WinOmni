namespace OmniUI.Helpers
{
    using System.Windows;
    using System.Windows.Threading;

    public class ApplicationHelper : IApplicationHelper
    {
        #region Static Fields

        private static IApplicationHelper _instance;

        #endregion

        #region Constructors and Destructors

        protected ApplicationHelper()
        {
        }

        #endregion

        #region Public Properties

        public static IApplicationHelper Instance
        {
            get
            {
                return _instance ?? new ApplicationHelper();
            }
            set
            {
                _instance = value;
            }
        }

        public Dispatcher Dispatcher
        {
            get
            {
                return Application.Current.Dispatcher;
            }
        }

        #endregion

        #region Public Methods and Operators

        public object FindResource(string key)
        {
            return Application.Current.FindResource(key);
        }

        public void Shutdown()
        {
            Application.Current.Shutdown();
        }

        #endregion
    }
}
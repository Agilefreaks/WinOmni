namespace CustomizedClickOnce.Uninstall
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Threading;
    using CustomizedClickOnce.Common;
    using Omnipaste;

    public static class Program
    {
        #region Static Fields

        private static Mutex _instanceMutex;

        #endregion

        #region Public Methods and Operators

        [STAThread]
        public static void Main()
        {
            try
            {
                bool createdNew;
                _instanceMutex = new Mutex(
                    true, @"Local\" + Assembly.GetExecutingAssembly().GetType().GUID, out createdNew);
                if (createdNew)
                {
                    StartUninstallProcess();
                }
                else
                {
                    _instanceMutex = null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        #endregion

        #region Methods

        private static void ReleaseMutex()
        {
            if (_instanceMutex == null)
            {
                return;
            }

            _instanceMutex.ReleaseMutex();
            _instanceMutex.Close();
            _instanceMutex = null;
        }

        private static void StartUninstallProcess()
        {
            var clickOnceHelper = new ClickOnceHelper(ApplicationInfoFactory.Create());
            clickOnceHelper.Uninstall();

            ReleaseMutex();
        }

        #endregion
    }
}
namespace Omnipaste.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Threading;
    using Microsoft.Practices.ServiceLocation;
    using Ninject;
    using Omni;
    using OmniUI.Helpers;

    public class ApplicationHelper : IApplicationHelper
    {
        #region Static Fields

        private static IApplicationHelper _instance;

        private readonly IList<IStartable> _backgroundServices;

        #endregion

        #region Constructors and Destructors

        protected ApplicationHelper()
        {
            _backgroundServices = new List<IStartable>();
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

        public void Shutdown()
        {
            Application.Current.Shutdown();
        }

        public void StartBackgroundService<TType>()
            where TType : IStartable
        {
            //Since the service implements IStartable it will be started as soon as it's activated
            _backgroundServices.Add(ServiceLocator.Current.GetInstance<TType>());
        }

        public void StopBackgroundProcesses()
        {
            var allStartedServices = ServiceLocator.Current.GetAllInstances<IStartable>()
                .Concat(_backgroundServices).Distinct();
            foreach (var service in allStartedServices)
            {
                service.Stop();
            }
            ServiceLocator.Current.GetInstance<IOmniService>().Dispose();
        }

        #endregion
    }
}
namespace Omnipaste.Activity
{
    using Ninject;
    using Omnipaste.Activity.Models;
    using Omnipaste.Services;

    public class ActivityViewModelFactory : IActivityViewModelFactory
    {
        #region Fields

        private readonly IUiRefreshService _uiRefreshService;

        #endregion

        #region Constructors and Destructors

        public ActivityViewModelFactory(IUiRefreshService uiRefreshService)
        {
            _uiRefreshService = uiRefreshService;
        }

        #endregion

        #region Public Properties

        [Inject]
        public IKernel Kernel { get; set; }

        #endregion

        #region Public Methods and Operators

        public IActivityViewModel Create(Models.Activity activity)
        {
            IActivityViewModel result;
            switch (activity.Type)
            {
                case ActivityTypeEnum.Call:
                case ActivityTypeEnum.Message:
                    result = new ContactRelatedActivityViewModel(_uiRefreshService);
                    break;
                default:
                    result = new ActivityViewModel(_uiRefreshService);
                    break;
            }

            result.Model = activity;
            Kernel.Inject(result);

            return result;
        }

        #endregion
    }
}